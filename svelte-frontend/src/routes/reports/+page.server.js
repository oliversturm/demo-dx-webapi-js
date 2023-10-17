import { loadData } from '$lib/dataLoading.js';

export function load({ fetch, url }) {
	return loadData(fetch, url, 'ReportDataV2', 'DevExpress.Persistent.BaseImpl.EF.ReportDataV2');
}
