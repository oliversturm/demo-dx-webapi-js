<script>
	import DataTable from '$lib/DataTable.svelte';

	export let data;
	$: ({ dataSource } = data);

	const fields = {
		Name: { class: 'text-left' },
		Price: { class: 'text-right' }
	};
</script>

<h2 class="font-bold text-xl">Data: Sale Products</h2>

<DataTable {dataSource} {fields} />
