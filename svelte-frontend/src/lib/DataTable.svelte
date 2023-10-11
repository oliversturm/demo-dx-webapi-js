<script>
	import { createEventDispatcher } from 'svelte';
	import FilterEditor from './FilterEditor.svelte';

	const dispatch = createEventDispatcher();
	const refresh = (newState) => {
		dispatch('displayStateChanged', newState);
	};

	export let dataSource;
	export let fields;
	export let schema = {};
	export let displayState = {
		sort: undefined,
		desc: false,
		filters: {}
	};
	export let editBaseUrl = null;
	export let extraRowActionButtons = null;

	const headerClick = (field) => () => {
		const newSort = field;
		const newDesc = displayState.sort === newSort ? !displayState.desc : false;
		refresh({ ...displayState, sort: newSort, desc: newDesc });
	};

	const newFilterValue =
		(fieldName) =>
		({ detail: newValue }) => {
			refresh({
				...displayState,
				filters: {
					...displayState.filters,
					// Set the value for this field's filter, keeping
					// the pre-configured filter type
					[fieldName]: { filter: fields[fieldName].filter, value: newValue }
				}
			});
		};
</script>

<table class="border-separate w-full">
	<tr>
		{#each Object.keys(fields) as f}
			<th class={fields[f].class} on:click={headerClick(f)}
				>{schema[f] || f} {displayState.sort === f ? (displayState.desc ? '↓' : '↑') : ''}</th
			>
		{/each}
		<th class="action">
			{#if editBaseUrl}
				<a href={editBaseUrl} alt="Create"><span class="fa fa-star-o" /></a>
			{:else}
				<div>&nbsp;</div>
			{/if}
		</th>
	</tr>
	<tr class="filterRow">
		{#each Object.keys(fields) as f}
			{@const field = fields[f]}
			{@const filter = field.filter}
			<td class={field.class}>
				{#if filter && filter !== 'none'}
					<FilterEditor
						{filter}
						value={displayState.filters[f]?.value}
						on:newValueConfirmed={newFilterValue(f)}
					/>
				{/if}
			</td>
		{/each}
		<td class="action">
			<div>&nbsp;</div>
		</td>
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
					<td class="action">
						{#if editBaseUrl}
							<a href="{editBaseUrl}/{item[schema['$$idField']]}" alt="Edit"
								><span class="fa fa-edit" /></a
							>
						{/if}
						{#if extraRowActionButtons}
							<svelte:component this={extraRowActionButtons} row={item} />
						{/if}
					</td>
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
	tr.filterRow > td {
		@apply bg-red-200;
	}
	.action {
		@apply bg-green-200 text-center flex gap-1;
	}
	.action a {
		@apply border-2 rounded bg-white px-2 py-0.5 hover:bg-orange-200;
	}
</style>
