<script>
	import '../app.css';
	import 'fork-awesome/css/fork-awesome.css';

	export let data;
	$: ({ userName, hasJwt } = data);
</script>

<h1 class="text-2xl mt-4 mb-8 text-center">DevExpress Web API Service from a Svelte App</h1>

<div class="w-9/12 max-w-prose m-auto">
	<nav class="mb-8 flex gap-2">
		<a href="/saleProducts">Sale Products</a>
		<a href="/reports">Reports</a>
		<a class="ml-auto" href="/account">
			{#if userName}
				<span class="fa fa-user-circle-o mr-4" />Account "{userName}"{hasJwt ? ' (JWT)' : ''}
			{:else}
				<span class="fa fa-unlock mr-4" />Log in
			{/if}
		</a>
	</nav>

	<slot />
</div>

<style lang="postcss">
	a {
		@apply bg-blue-200 px-4 py-1 rounded hover:bg-orange-200;
	}
</style>
