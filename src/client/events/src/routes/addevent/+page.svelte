<script lang="ts">
  import { enhance } from '$app/forms';
  import EventForm from '$lib/components/EventForm.svelte';
  import { CardData, type cardItem } from '$lib/shared/CardData.svelte';
  import { FormType } from '$lib/shared/formtype.js';
  import { page } from '$app/state'
  import type { PageProps } from '../$types';
	import { goto } from '$app/navigation';
  import {events, toDTO} from '$lib/shared/CardDTO.svelte'

    let {data, form }: {data:PageProps, form: any} = $props()

  type CardPost = {
    id: number;
    data: CardData;
  };

  let objectId = null;
  const onsuccess = async (result: CardPost)=>{

      await goto('/', {state:{successfullUpdate: result}})
  }

</script>


<EventForm {onsuccess}  formtype = {FormType.Create} {objectId} form={form} children={null}/>
