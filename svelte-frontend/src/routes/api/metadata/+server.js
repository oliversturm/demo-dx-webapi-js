import { json } from '@sveltejs/kit';
import { XMLParser } from 'fast-xml-parser';

// Could prerender to prevent extra roundtrips to the XML data
// export const prerender = true;

const parser = new XMLParser({
	ignoreAttributes: false,
	attributeNamePrefix: '',
	attributesGroupName: '@_attributes'
});

export async function GET({ fetch }) {
	const result = await fetch('http://webapi:5273/api/odata/$metadata', {
		headers: {
			Accept: 'application/xml'
		}
	})
		.then((res) => res.text())
		.then((xmlString) => parser.parse(xmlString));

	return json(result);
}
