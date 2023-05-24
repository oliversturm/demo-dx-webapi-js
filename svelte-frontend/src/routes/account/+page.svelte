<script>
	import { enhance } from '$app/forms';

	export let data;
	$: ({ userName } = data);

	export let form;
</script>

{#if userName}
	<p class="font-bold mb-8">
		You are logged in as "{userName}"
	</p>
	<form method="POST" action="?/logout" use:enhance>
		<button type="submit">Logout</button>
	</form>
{:else}
	<form method="POST" action="?/login" use:enhance>
		<label for="userName">Username</label>
		<input type="text" name="userName" id="userName" value={form?.userName || ''} />
		<label for="password">Password</label>
		<input type="password" name="password" id="password" />
		<button type="submit">Login</button>

		{#if form?.error}
			<div class="error">{form.error}</div>
		{/if}
	</form>
{/if}

<style lang="postcss">
	.error {
		@apply font-bold border bg-red-200 p-2 rounded;
	}
	button {
		@apply bg-orange-400 mt-4 mr-auto px-4 py-1 rounded hover:bg-orange-200;
	}
	form {
		@apply flex flex-col;
	}
	input {
		@apply border mb-4;
	}
	label {
		@apply text-sm;
	}
</style>
