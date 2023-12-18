import { json } from '@sveltejs/kit';
import mergeAll from 'lodash/fp/mergeAll';

const getPropertyInfo = (() => {
	const oneFieldKey = (et) => {
		if (et['$Key'] && Array.isArray(et['$Key']) && et['$Key'].length === 1) {
			return et['$Key'][0];
		} else return;
	};

	const splitTypeName = (fullTypeName) => {
		const lastDot = fullTypeName.lastIndexOf('.');
		return [fullTypeName.slice(0, lastDot), fullTypeName.slice(lastDot + 1)];
	};

	const recurse = (data, namespace, entityName) => {
		if (data[namespace] && data[namespace][entityName]) {
			const baseResult = data[namespace][entityName].$BaseType
				? recurse(data, ...splitTypeName(data[namespace][entityName].$BaseType))
				: { propertyNames: [], idField: '' };

			return {
				propertyNames: [
					baseResult.propertyNames,
					Object.keys(data[namespace][entityName]).filter((k) => !k.startsWith('$'))
				].flat(),
				idField: oneFieldKey(data[namespace][entityName]) || baseResult.idField
			};
		} else {
			console.error(`No schema found for namespace ${namespace} and entity ${entityName}`);
			return { propertyNames: [], idField: '' };
		}
	};

	let jsonData;

	return (fetch, namespace, entityName) =>
		(jsonData
			? Promise.resolve(jsonData)
			: fetch('/api/metadata')
					.then((res) => res.json())
					.then((data) => {
						jsonData = data;
						return data;
					})
		).then((data) => recurse(data, namespace, entityName));
})();

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
		.then((classCaption) => (classCaption === className ? '' : classCaption))
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
