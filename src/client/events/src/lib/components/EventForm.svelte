<script lang='ts'>
    import { enhance } from '$app/forms';
    import { FormType } from '$lib/shared/formtype';
    import type { CardPost } from '$lib/shared/CardPost'
	import CreateEventForm, { type ActionSuccess } from './CreateEventForm.svelte';
	import { page } from '$app/state';
	import { CardData } from '$lib/shared/CardData.svelte';


    let { 
            children, 
            form, 
            formtype, 
            objectId,
            onsuccess,
     } 
    : {
        objectId: number | null, 
        formtype: FormType, 
        children: any | null, 
        form: any,
        onsuccess: (x: CardPost)=>void,
    } = $props();

    const addEventOpts = {
        kind: 'event',
        mode: 'create',
        objectId:null,
        onsuccess: (e: ActionSuccess)=>{
            const start = new Date(Date.parse(e.dto.dateStart))
            const end = new Date(Date.parse(e.dto.dateEnd))
            console.log(e.dto)
            console.log(e)
            console.log(`!!!!!!!!!!!!!!!!!!!!!!!!!!!!${e.dto.dateStart}`)
            console.log(`!!!!!!!!!!!!!!!!!!!!!!!!!!!!${e.dto.dateStart}`)


            console.log(`!!!!!!!!!!!!!!!!!!!!!!!!!!!!${start.getDate()}`)
            console.log(`!!!!!!!!!!!!!!!!!!!!!!!!!!!!${end.getDate()}`)

            const result = {
                data: new CardData(
                    start,
                    end,
                    e.dto.covers as string[],
                    e.dto.features as string[] | null,
                    e.dto.eventType as string | null, 
                    e.dto.eventName as string),
                id: e.dto.id,

            }
            onsuccess(result)
        }
    } as const;

</script>

{#if formtype == FormType.Create}
    <CreateEventForm {form} {...addEventOpts}/>
{/if}
