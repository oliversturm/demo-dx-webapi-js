<script>
	import { enhance } from '$app/forms';

	export let data;
	$: ({ product, schema } = data);

	export let form;
</script>

{#if product.ID}
	<h2>Edit {schema['$$classCaption']} "{product.Name}"</h2>
{:else}
	<h2>Create new product</h2>
{/if}

<form method="post" use:enhance>
	<input type="hidden" name="ID" value={product.ID || ''} />
	<table class="border-y-2 w-full">
		<tr>
			<td><label for="name">{schema.Name}</label></td>
			<td><input type="text" id="name" name="Name" bind:value={product.Name} /></td>
		</tr>
		<tr>
			<td><label for="price">{schema.Price}</label></td>
			<td>
				<input
					class="text-right"
					type="number"
					id="price"
					name="Price"
					step={0.01}
					bind:value={product.Price}
				/>
			</td>
		</tr>
	</table>
	{#if form?.error}
		<div class="bg-red-200 rounded w-full p-2 m-2 whitespace-pre-line">
			{form.error}
		</div>
	{/if}
	<div class="flex mt-4">
		<button class="ml-auto bg-green-200 px-4 py-1 rounded hover:bg-red-200" type="submit">
			Save
		</button>
	</div>
</form>

<style lang="postcss">
	input {
		@apply border px-2 w-full;
	}
	label {
		@apply mx-2;
	}
	td {
		@apply py-2;
	}
</style>
