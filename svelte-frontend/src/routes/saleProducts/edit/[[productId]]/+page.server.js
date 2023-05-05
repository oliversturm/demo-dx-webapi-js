import { redirect, fail } from '@sveltejs/kit';

export function load({ fetch, params }) {
	let product = { Name: '', Price: 0.0 };
	if (params.productId) {
		product = fetch(`http://webapi:5273/api/odata/SaleProduct/${params.productId}`).then((res) =>
			res.json()
		);
	}
	const schema = fetch('/api/schema/XAFApp.Module.BusinessObjects.SaleProduct').then((res) =>
		res.json()
	);
	return { product, schema };
}

export const actions = {
	default: async ({ request, fetch }) => {
		const formData = Object.fromEntries(await request.formData());
		const postData = { Name: formData.Name, Price: parseFloat(formData.Price) || 0 };

		if (!formData.ID) {
			const response = await fetch(`http://webapi:5273/api/odata/SaleProduct`, {
				method: 'POST',
				headers: { Accept: 'application/json', 'Content-Type': 'application/json' },
				body: JSON.stringify(postData)
			});
			if (response.ok) {
				const json = await response.json();
				throw redirect(303, `/saleProducts/edit/${json.ID}`);
			} else {
				return fail(400, { error: await response.text() });
			}
		} else {
			const response = await fetch(`http://webapi:5273/api/odata/SaleProduct/${formData.ID}/`, {
				method: 'PATCH',
				headers: { Accept: 'application/json', 'Content-Type': 'application/json' },
				body: JSON.stringify(postData)
			});
			if (response.ok) {
				return null;
			} else {
				return fail(400, { error: await response.text() });
			}
		}
	}
};
