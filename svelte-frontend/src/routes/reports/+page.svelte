<script>
	import { page } from '$app/stores';

	import DataTable from '$lib/DataTable.svelte';
	import ReportRowActionButtons from '$lib/ReportRowActionButtons.svelte';
	import { displayStateChanged } from '$lib/dataLoading.js';

	export let data;
	$: ({ displayState, dataSource: reports, schema, error } = data);

	const fields = {
		DisplayName: { class: 'text-left', filter: 'string' },
		DataTypeName: { class: 'text-left', filter: 'string' }
	};
	const classCaptionPlaceholder = 'Report';
</script>

<h2 class="font-bold text-xl">
	Data: {schema ? schema['$$classCaption'] : classCaptionPlaceholder}
</h2>

{#if error}
	<div class="error">{error}</div>
{:else}
	<DataTable
		dataSource={reports}
		{fields}
		{displayState}
		{schema}
		on:displayStateChanged={displayStateChanged($page.url.pathname)}
		extraRowActionButtons={ReportRowActionButtons}
	/>
{/if}

<style lang="postcss">
	.error {
		@apply font-bold border bg-red-200 p-2 mt-4 rounded;
	}
</style>
