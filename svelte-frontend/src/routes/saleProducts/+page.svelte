<script>
	import DataTable from '$lib/DataTable.svelte';

	export let data;
	$: ({ dataSource, schema, error } = data);

	const fields = {
		Name: { class: 'text-left' },
		Price: { class: 'text-right' }
	};

	const classCaptionPlaceholder = 'Sale Products';
</script>

<h2 class="font-bold text-xl">
	Data: {schema ? schema['$$classCaption'] : classCaptionPlaceholder}
</h2>

{#if error}
	<div class="error">{error}</div>
{:else}
	<DataTable {dataSource} {fields} {schema} />
{/if}

<style lang="postcss">
	.error {
		@apply font-bold border bg-red-200 p-2 rounded;
	}
</style>
