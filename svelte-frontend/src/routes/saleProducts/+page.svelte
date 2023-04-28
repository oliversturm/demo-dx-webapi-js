<script>
	import DataTable from '$lib/DataTable.svelte';
	import { displayStateChanged } from '$lib/dataLoading.js';
	import { page } from '$app/stores';

	export let data;
	$: ({ dataSource, schema, displayState, error } = data);

	const fields = {
		Name: { class: 'text-left', filter: 'string' },
		Price: { class: 'text-right', filter: 'number' }
	};

	const classCaptionPlaceholder = 'Sale Products';
</script>

<h2 class="font-bold text-xl">
	Data: {schema ? schema['$$classCaption'] : classCaptionPlaceholder}
</h2>

{#if error}
	<div class="error">{error}</div>
{:else}
	<DataTable
		{dataSource}
		{fields}
		{schema}
		{displayState}
		on:displayStateChanged={displayStateChanged($page.url.pathname)}
		editBaseUrl="/saleProducts/edit"
	/>
{/if}

<style lang="postcss">
	.error {
		@apply font-bold border bg-red-200 p-2 rounded;
	}
</style>
