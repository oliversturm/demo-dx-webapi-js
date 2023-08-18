export function handleFetch({ event, request, fetch }) {
	// As per https://kit.svelte.dev/docs/hooks#server-hooks-handlefetch ,
	// add the cookie that is not passed through automatically
	// because the request accesses the webapi server on a different
	// port.
	if (request.url.startsWith('http://webapi:5273/api')) {
		const jwt = event.cookies.get('webapiDemoJwt');
		if (jwt) {
			// We are working in JWT mode, so set the Authorization header
			request.headers.set('Authorization', `Bearer ${jwt}`);
		} else {
			request.headers.set('cookie', event.request.headers.get('cookie'));
		}
	}
	return fetch(request);
}
