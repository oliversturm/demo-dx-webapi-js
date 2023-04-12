import { json } from '@sveltejs/kit';
import mergeAll from 'lodash/fp/mergeAll';

const getObjectOrList = (o, pred) => {
	if (!o) return undefined;

	// It's possible that o is an array of objects, but it could also just be one
	// object. The predicate must either match the object, or we use it to find
	// the object in the array.

	return Array.isArray(o) ? o.find(pred) : pred(o) ? o : undefined;
};

const getPropertyInfo = (fetch, namespace, entityName) =>
	fetch('/api/metadata')
		.then((res) => res.json())
		.then((data) => {
			const schemaParent = data['edmx:Edmx']['edmx:DataServices']['Schema'];

			const schema = getObjectOrList(
				schemaParent,
				(el) => el['@_attributes']?.Namespace === namespace
			);
			if (!schema)
				throw new Error(`No schema found for namespace ${namespace} and entity ${entityName}`);

			const et = getObjectOrList(
				schema.EntityType,
				(el) => el['@_attributes']?.Name === entityName
			);
			if (!et)
				throw new Error(`No entity type found for namespace ${namespace} and entity ${entityName}`);

			// I guess technically we should check that the Property field is an array
			// and not an object again. But I won't bother because I expect the implementation
			// of the service to change so it can deliver JSON -- and the current weird
			// structure is only because the service is returning XML.
			const result = {
				propertyNames: et.Property?.map((p) => p['@_attributes']?.Name).filter((pn) => !!pn),
				idField: et.Key?.PropertyRef['@_attributes']?.Name
			};
			return result;
		})
		.catch((err) => {
			console.error(err);
			return { propertyNames: [], idField: '' };
		});

// These functions use encodeURIComponent only so that
// we don't accidentally send an invalid URL. Valid
// names of classes and members should not need
// encoding.
const getClassCaptionPromise = (fetch, className, lang) =>
	fetch(
		`http://webapi:5273/api/Localization/ClassCaption?classFullName=${encodeURIComponent(
			className
		)}`,
		lang
			? {
					headers: {
						'Accept-Language': lang
					}
			  }
			: undefined
	)
		.then((res) => res.text())
		.then((classCaption) => ({ $$classCaption: classCaption }));

const getMemberCaptionPromise = (fetch, className, memberName, lang) =>
	fetch(
		`http://webapi:5273/api/Localization/MemberCaption?classFullName=${className}&memberName=${memberName}`,
		lang
			? {
					headers: {
						'Accept-Language': lang
					}
			  }
			: undefined
	)
		.then((res) => res.text())
		.then((memberCaption) => ({ [memberName]: memberCaption }));

export async function GET({ fetch, params }) {
	// className is expected to be XAFProject.Module.BusinessObjects.Thing -- or something like that
	const { className, lang = '' } = params;
	const namespace = className.slice(0, className.lastIndexOf('.'));
	const entityName = className.slice(className.lastIndexOf('.') + 1);

	const { propertyNames, idField } = await getPropertyInfo(fetch, namespace, entityName);

	const promises = [
		Promise.resolve({ $$idField: idField }),
		getClassCaptionPromise(fetch, className, lang),
		...propertyNames.map((pn) => getMemberCaptionPromise(fetch, className, pn, lang))
	];

	const result = await Promise.all(promises).then(mergeAll);

	return json(result);
}
