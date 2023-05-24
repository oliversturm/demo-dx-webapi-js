export function handleFetch({ event, request, fetch }) {
	// As per https://kit.svelte.dev/docs/hooks#server-hooks-handlefetch ,
	// add the cookie that is not passed through automatically
	// because the request accesses the webapi server on a different
	// port.
	if (request.url.startsWith('http://webapi:5273/api')) {
		request.headers.set('cookie', event.request.headers.get('cookie'));
	}
	return fetch(request);
}
