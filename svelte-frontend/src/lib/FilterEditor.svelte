<script>
	import { createEventDispatcher } from 'svelte';

	const dispatch = createEventDispatcher();

	const newValueConfirmed = () => {
		dispatch('newValueConfirmed', value);
	};

	const onKeyDown = (e) => {
		if (e.key === 'Enter') newValueConfirmed();
	};

	const resetValue = () => {
		value = undefined;
		newValueConfirmed();
	};

	export let filter;
	// value is bound -- can be used through binding
	// in the parent, or alternatively once a new
	// value is confirmed and the event is raised
	export let value;
</script>

<!-- Only string filter supported at this time -->
{#if filter === 'string'}
	<div class="flex items-center">
		<span class="fa fa-filter mr-2" /><input
			class="flex-grow px-2"
			type="text"
			placeholder="Contains ..."
			bind:value
			on:keypress={onKeyDown}
		/><button
			class="ml-2 bg-green-200 px-2 py-1 disabled:bg-gray-200 fa fa-check"
			disabled={!value}
			on:click={newValueConfirmed}
		/><button
			class="ml-1 bg-white text-red-500 px-2 py-1 disabled:text-gray-600 disabled:bg-gray-200 fa fa-times"
			disabled={!value}
			on:click={resetValue}
		/>
	</div>
{/if}
