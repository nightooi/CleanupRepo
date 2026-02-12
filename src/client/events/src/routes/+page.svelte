<script lang="ts">
    import Eventlist  from "$lib/components/Eventlist.svelte"
	import EventCard from "$lib/components/EventCard.svelte";
	import { CardData, type cardItem } from "$lib/shared/CardData.svelte";
    import { page } from "$app/state";
	import type { CardPost } from "$lib/shared/CardPost";
	import { replaceState } from "$app/navigation";
	import { fromDTO, toDTO, type CardPostDTO } from "$lib/shared/CardDTO.svelte";
    import { events } from "$lib/shared/CardDTO.svelte";
    import { type EventData } from "$lib/shared/CardDTO.svelte";

let { navigation } = $props() as { page: any, navigation: any}



const addDays =(days:number) => {

    return days * (60*60*1000*24)
}

let currObj = {
    id: 2,
    data: new CardData(new Date(Date.now()), new Date(Date.now() + addDays(3)), ['./placeHolder.jpg'], ['FoodTrucks'], 'food-market', 'FetTorsdag i göteborg'),
    width: null,
    height: null,
} as cardItem;

let other ={
    id: 1,
    data: new CardData(new Date(Date.now() + addDays(3)), new Date(Date.now() + addDays(6)), ['./placeHolder.jpg'], ['swedish house maffia'], 'house-music concert', 'shm tour 2026 show'),
    width: null,
    height: null,
} as cardItem

let third ={
    id: 0,
    data: new CardData(new Date(Date.now() + addDays(6)), new Date(Date.now() + addDays(9)), ['./placeHolder.jpg'], ['native folk dances'], 'native culture', 'götinge festival'),
    width: null,
    height: null,
}

let handled = $state(0);


async function getItems(){
    let res = await fetch('http://localhost:5133/events/Events', {
        method: 'GET',
        headers: {'content-type':'application/json'}});

    if(res.ok)
    {
        const data = await res.json() as EventData[];
        const transformed = data.map(x => eventDataToCardPostDTO(x));
        
        // Add new events that don't already exist
        for (const item of transformed) {
            if (!events.find(e => e.id === item.id)) {
                events.push(item);
            }
        }
    }
}

function eventDataToCardPostDTO(
    e: EventData,
    opts?: { width?: string | null; height?: string | null }
): CardPostDTO {
    return {
        id: parseInt(e.id as unknown as string) || Math.random(),
        width: opts?.width ?? null,
        height: opts?.height ?? null,
        data: {
            eventName: e.eventName ?? '',
            dateStart: e.eventStart,
            dateEnd: e.eventEnd,
            covers: e.covers ?? [],
            eventType: e.eventType ?? '',
            features: e.features ?? []
        }
    };
}

let items = $derived(events.map(x=> fromDTO(x)))

// Fetch events on mount
$effect(() => {
    getItems();
});

$effect(()=>{
    const incoming = page.state?.successfullUpdate ? page.state?.successfullUpdate : null;
    if(!incoming) return;

    const result = (incoming as CardPost)

    if(handled == result.id) return;

    handled = result.id;

    if(events.find(x=> x.id == result.id)) return;
        events.unshift(toDTO({id: result.id, width:null, height:null, data: result.data }))

    replaceState(page.url, {})
})
$inspect(items)


</script>
<a href='./addevent'>Add new Event</a>
<Eventlist bind:items>
    {#snippet row({data}: {data: cardItem})}
        <EventCard {...data as cardItem}>
            <p>Child of EventCard</p>
        </EventCard>
    {/snippet}
    <p>EventList child</p>
</Eventlist>


<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://svelte.dev/docs/kit">svelte.dev/docs/kit</a> to read the documentation</p>
