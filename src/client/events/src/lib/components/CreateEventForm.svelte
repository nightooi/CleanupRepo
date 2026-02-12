<script lang="ts">
  import { enhance } from '$app/forms';
  import { stockImages } from '$lib/shared/stockImages';

  export type CrudMode = 'create' | 'update' | 'delete';
  export type FormKind = 'event' | 'category';

  export type ActionSuccess = { dto?: any; id?: number; ok?: boolean };
  export type ActionFailure = { errors?: Record<string, string>; values?: Record<string, any> };

  type Props = {
    form?: ActionFailure & ActionSuccess;

    mode?: CrudMode;               // create/update/delete
    kind: FormKind;                // event/category
    options?: string[];            // field toggles
    objectId?: number | null;      // for update/delete

    action?: string;
    method?: 'POST';

    onsuccess?: (payload: ActionSuccess) => void;
    onfailure?: (payload: ActionFailure) => void;

    children?: any;
  };

  let {
    form,
    mode = 'create',
    kind,
    options = [],
    objectId = null,
    action,
    method = 'POST',
    onsuccess,
    onfailure,
    children
  }: Props = $props();

  const has = (opt: string) => options.length === 0 || options.includes(opt);

  let actionUrl = $derived(action ?? `?/${mode}`);
  
  let values = $derived(form?.values ?? {
    eventName: "Food-market",
    eventType: "Market-Type",
    dateStart: Date.now(),
    dateEnd: new Date(Date.now() + 24 * 60 * 60 * 1000),
    features: 'Food Stalls',
    categoryName: 'Food Event'
  });
  let errors = $derived(form?.errors ?? {});

  //disable inputs for delete
  let isDelete = $derived(mode === 'delete');

  function handleEnhance() {
    return async ({ result }: any) => {

      if (result.type === 'success') {

        onsuccess?.(result.data as ActionSuccess);
        return;
      }

      if (result.type === 'failure') {

        onfailure?.(result.data as ActionFailure);
        return;
      }

    };
  }
</script>

<form
  method={method}
  action={actionUrl}
  use:enhance={handleEnhance}
  class="space-y-4 max-w-xl"
>
  {#if objectId != null}
    <input type="hidden" name="id" value={objectId} />
  {/if}

  {#if kind === 'event'}
    {#if has('eName')}
      {@render fieldInput({
        name: 'eventName',
        label: 'Event name',
        placeholder: 'Fat Thursday in Gothenburg',
        value: values.eventName,
        error: errors.eventName,
        required: true,
        disabled: isDelete
      })}
    {/if}

    {#if has('eType')}
      {@render fieldInput({
        name: 'eventType',
        label: 'Event type (optional)',
        placeholder: 'food-market',
        value: values.eventType,
        error: errors.eventType,
        required: false,
        disabled: isDelete
      })}
    {/if}

    {#if has('sDate')}
      {@render fieldInput({
        name: 'dateStart',
        label: 'Start date',
        type: 'date',
        value: values.dateStart,
        error: errors.dateStart,
        required: true,
        disabled: isDelete
      })}
    {/if}

    {#if has('eDate')}
      {@render fieldInput({
        name: 'dateEnd',
        label: 'End date',
        type: 'date',
        value: values.dateEnd,
        error: errors.dateEnd,
        required: true,
        disabled: isDelete
      })}
    {/if}

    {#if has('cover')}
      {@render fieldImageSelect({
        name: 'covers',
        label: 'Cover Image',
        value: values.covers,
        error: errors.covers,
        required: true,
        disabled: isDelete
      })}
    {/if}

    {#if has('features')}
      {@render fieldTextarea({
        name: 'features',
        label: 'Event Features (optional, one per line or comma-separated)',
        placeholder: 'FoodTrucks\nLiveMusic',
        value: values.features,
        error: errors.features,
        required: false,
        disabled: isDelete,
        rows: 2
      })}
    {/if}

  {:else if kind === 'category'}
    {#if has('cName')}
      {@render fieldInput({
        name: 'categoryName',
        label: 'Add category',
        placeholder: 'Traditional Instruments Music',
        value: values.categoryName,
        error: errors.categoryName,
        required: true,
        disabled: isDelete
      })}
    {/if}

    {#if has('altC')}
      {@render fieldInput({
        name: 'alterName',
        label: 'New name',
        placeholder: 'New category name',
        value: values.alterName,
        error: errors.alterName,
        required: true,
        disabled: isDelete
      })}
    {/if}
  {/if}

  <div class="flex items-center gap-3">
    <button
      class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white disabled:opacity-50"
      type="submit"
    >
      {#if mode === 'create'}Create{/if}
      {#if mode === 'update'}Update{/if}
      {#if mode === 'delete'}Delete{/if}
    </button>

    {@render children?.()}
  </div>
</form>

{#snippet fieldInput({ name, label, placeholder = '', value = '', error = '', type = 'text', required = false, disabled = false })}
  <div>
    <label class="block text-sm font-medium" for={name}>{label}</label>
    <input
      id={name}
      name={name}
      type={type}
      class="mt-1 w-full rounded-lg border px-3 py-2"
      placeholder={placeholder}
      value={value ?? ''}
      {required}
      {disabled}
    />
    {#if error}
      <p class="mt-1 text-sm text-red-600">{error}</p>
    {/if}
  </div>
{/snippet}

{#snippet fieldTextarea({ name, label, placeholder = '', value = '', error = '', required = false, disabled = false, rows = 2 })}
  <div>
    <label class="block text-sm font-medium" for={name}>{label}</label>
    <textarea
      id={name}
      name={name}
      class="mt-1 w-full rounded-lg border px-3 py-2"
      placeholder={placeholder}
      {rows}
      {required}
      {disabled}
    >{value ?? ''}</textarea>
    {#if error}
      <p class="mt-1 text-sm text-red-600">{error}</p>
    {/if}
  </div>
{/snippet}

{#snippet fieldImageSelect({ name, label, value = '', error = '', required = false, disabled = false })}
  <div>
    <label class="block text-sm font-medium mb-2" for={name}>{label}</label>
    <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
      {#each stockImages as image}
        <label 
          class="image-option relative cursor-pointer rounded-xl overflow-hidden border-2 transition-all"
          class:selected={value === image.url || (!value && image.id === 'default')}
          class:opacity-50={disabled}
        >
          <input
            type="radio"
            name={name}
            bind:value={image.url}
            checked={value === image.url || (!value && image.id === 'default')}
            {required}
            {disabled}
            class="sr-only"
          />
          <div class="aspect-video bg-cover bg-center" style:background-image="url({image.url})"></div>
          <div class="absolute inset-0 bg-gradient-to-t from-black/70 via-transparent to-transparent"></div>
          <span class="absolute bottom-1.5 left-2 right-2 text-xs font-medium text-white truncate">
            {image.name}
          </span>
          <div class="check-indicator absolute top-2 right-2 w-5 h-5 rounded-full bg-blue-500 flex items-center justify-center opacity-0 transition-opacity">
            <svg class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
            </svg>
          </div>
        </label>
      {/each}
    </div>
    {#if error}
      <p class="mt-2 text-sm text-red-600">{error}</p>
    {/if}
  </div>
{/snippet}

<style>
  .image-option {
    border-color: transparent;
  }
  .image-option:hover {
    border-color: rgba(59, 130, 246, 0.5);
  }
  .image-option.selected {
    border-color: #3b82f6;
    box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2);
  }
  .image-option.selected .check-indicator {
    opacity: 1;
  }
</style>
