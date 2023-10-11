<script>
	import { onMount } from 'svelte';

	export let data;
	$: ({ pdfDocumentSerialized } = data);

	$: pdfDocument = new Blob(
		[new Uint8Array([...atob(pdfDocumentSerialized)].map((char) => char.charCodeAt(0)))],
		{ type: 'application/pdf' }
	);

	onMount(() => {
		if (pdfDocument) {
			const url = URL.createObjectURL(pdfDocument);
			console.log(`Got document, URL created: ${url}`);
			iframe.src = url;
		}
	});
	let iframe;
</script>

<div class="flex flex-col h-70vh">
	<iframe bind:this={iframe} class="w-full grow" title="Report Preview" />
</div>
