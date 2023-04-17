import { loadData } from '$lib/dataLoading.js';

export function load({ fetch, url }) {
	return loadData(fetch, url, 'SaleProduct', 'XAFApp.Module.BusinessObjects.SaleProduct');
}
