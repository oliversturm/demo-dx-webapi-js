export function load({ fetch }) {
	const odataUrl = `http://webapi:5273/api/odata/SaleProduct`;

	const dataSource = fetch(odataUrl)
		.then((res) => res.json())
		.then((res) => res.value);

	return { dataSource };
}
