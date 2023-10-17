<script>
  import { onMount } from "svelte";

  export let data;
  $: ({ pdfDocumentSerialized } = data);

  $: pdfDocument = new File(
    [new Uint8Array([...atob(pdfDocumentSerialized)].map((char) => char.charCodeAt(0)))],
    // Some browsers can use this filename property to associate with the
    // blob data. This is not supported by all browsers.
    "report.pdf",
    { type: "application/pdf" });

  onMount(() => {
    if (pdfDocument) {
      const url = URL.createObjectURL(pdfDocument);
      iframe.src = url;
    }
  });
  let iframe;
</script>

<div class="flex flex-col h-70vh">
  <iframe bind:this={iframe} class="w-full grow" title="Report Preview" />
</div>
