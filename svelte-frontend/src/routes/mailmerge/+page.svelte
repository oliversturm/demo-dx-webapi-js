<script>
  import { page } from "$app/stores";

  import DataTable from "$lib/DataTable.svelte";
  import MailMergeRowActionButtons from "$lib/MailMergeRowActionButtons.svelte";
  import { displayStateChanged } from "$lib/dataLoading.js";

  export let data;
  $: ({ displayState, dataSource: mailMergeItems, schema, error } = data);

  const fields = {
    Name: { class: "text-left", filter: "string" },
    TargetTypeFullName: { class: "text-left", filter: "string" }
  };
  const classCaptionPlaceholder = "Mail Merge Items";
</script>

<h2 class="font-bold text-xl">
  Data: {schema && schema['$$classCaption'] || classCaptionPlaceholder}
</h2>

{#if error}
  <div class="error">{error}</div>
{:else}
  <DataTable
    dataSource={mailMergeItems}
    {fields}
    {displayState}
    {schema}
    on:displayStateChanged={displayStateChanged($page.url.pathname)}
    extraRowActionButtons={MailMergeRowActionButtons}
  />
{/if}

<style lang="postcss">
    .error {
        @apply font-bold border bg-red-200 p-2 mt-4 rounded;
    }
</style>
