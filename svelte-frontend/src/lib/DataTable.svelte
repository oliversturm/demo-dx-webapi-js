<script>
	export let dataSource;
	export let fields;
	export let schema = {};
</script>

<table class="border-separate w-full">
	<tr>
		{#each Object.keys(fields) as f}
			<th class={fields[f].class}>{schema[f] || f}</th>
		{/each}
	</tr>
	{#await dataSource}
		<tr class="placeholder"><td colspan="2">Waiting for data</td></tr>
	{:then dataSourceContent}
		{#if dataSourceContent && Array.isArray(dataSourceContent)}
			{#each dataSourceContent as item}
				<tr>
					{#each Object.keys(fields) as f}
						<td class={fields[f].class}>{item[f]}</td>
					{/each}
				</tr>
			{:else}
				<tr class="placeholder empty">
					<td colspan={Object.keys(fields).length}>Empty list</td>
				</tr>
			{/each}
		{:else}
			<tr class="placeholder debugging">
				<td colspan={Object.keys(fields).length}>
					Unknown dataSourceContent content (debugging): <code
						>{JSON.stringify(dataSourceContent)}</code
					>
				</td>
			</tr>
		{/if}
	{:catch e}
		<tr class="placeholder error">
			<td colspan={Object.keys(fields).length}>Error: {e}</td>
		</tr>
	{/await}
</table>

<style lang="postcss">
	td,
	th {
		@apply px-2 py-1;
	}
	th {
		@apply bg-blue-300 cursor-pointer select-none;
	}
	td {
		@apply border border-gray-200;
	}
	tr.placeholder {
		@apply bg-gray-200;
	}
	tr.empty {
		@apply bg-yellow-200;
		td {
			@apply text-center p-4;
		}
	}
	tr.error {
		@apply bg-red-200 font-bold;
	}
</style>
