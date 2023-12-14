export function load({ fetch, params }) {
	// Using a server-side load mechanism is elegant in this
	// case because the call to the webapi docker container
	// is the same as elsewhere in the app (and authentication
	// cookie/JWT token is passed automatically). On the other
	// hand, we need to jump through a few hoops to serialize
	// the data for transfer to the client-side code in
	// +page.svelte.

	const { key, targetIds = '' } = params;

	const pdfDocumentSerialized = fetch(
		`http://webapi:5273/api/MailMerge/MergeDocument(${key})${targetIds ? `/${targetIds}` : ''}`
	)
		.then((res) => res.blob())
		.then((blob) => blob.arrayBuffer())
		.then((ab) => Buffer.from(ab).toString('base64'));

	return { pdfDocumentSerialized };
}
