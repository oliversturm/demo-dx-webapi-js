import { loadData } from '$lib/dataLoading.js';

export function load({ fetch, url }) {
	return loadData(
		fetch,
		url,
		'RichTextMailMergeData',
		'DevExpress.Persistent.BaseImpl.EF.RichTextMailMergeData'
	);
}
