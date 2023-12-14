import queryBuilder from 'odata-query';
import { loadData } from '$lib/dataLoading.js';

export async function load({ fetch, url }) {
	const qbParams = {
		filter: {
			TargetTypeFullName: 'XAFApp.Module.BusinessObjects.SaleProduct'
		}
	};
	const mailMergeDocumentId = await fetch(
		`http://webapi:5273/api/odata/RichTextMailMergeData${queryBuilder(qbParams)}`,
		{ redirect: 'manual' }
	)
		.then((res) => {
			if (res.ok) return res;
			throw new Error(`HTTP error, status: ${res.status}`);
		})
		.then((res) => res.json())
		.then((res) => res.value && res.value.length > 0 && res.value[0].ID);

	return await loadData(
		fetch,
		url,
		'SaleProduct',
		'XAFApp.Module.BusinessObjects.SaleProduct'
	).then((dataResult) => ({
		...dataResult,
		mailMergeDocumentId
	}));
}
