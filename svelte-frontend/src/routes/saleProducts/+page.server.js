export async function load({ fetch }) {
	const odataUrl = `http://webapi:5273/api/odata/SaleProduct`;

	const dataSource = fetch(odataUrl)
		.then((res) => {
			if (res.ok) return res;
			throw new Error(`HTTP error, status: ${res.status}`);
		})
		.then((res) => res.json())
		.then((res) => res.value);

	const schemaUrl = `/api/schema/XAFApp.Module.BusinessObjects.SaleProduct`;
	const schema = fetch(schemaUrl)
		.then((res) => {
			if (res.ok) return res;
			throw new Error(`HTTP error, status: ${res.status}`);
		})
		.then((res) => res.json());

	return await Promise.all([dataSource, schema])
		.then(([ds, sc]) => ({ dataSource: ds, schema: sc }))
		.catch((err) => {
			console.log(err);
			return { error: err.message };
		});
}
