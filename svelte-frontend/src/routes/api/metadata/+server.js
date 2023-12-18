import { json } from '@sveltejs/kit';

export async function GET({ fetch }) {
	const result = await fetch('http://webapi:5273/api/odata/$metadata?$format=json', {
		headers: {
			Accept: 'application/json'
		}
	}).then((res) => res.json());

	return json(result);
}
