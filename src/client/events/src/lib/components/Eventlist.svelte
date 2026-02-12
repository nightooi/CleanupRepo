<script lang="ts">
    import type{ IiterDisplay } from '$lib/shared/IteratableItem'
    import { asset } from '$app/paths'
	import { CardData } from '$lib/shared/CardData.svelte';

    // I need to decide who owns the list, wether it's the parent or the child..

    let { 
            children,
            items = $bindable(),
            row
        }: {children: any, items: IiterDisplay[], row: any }= $props()

</script>

<!--transitions and so on have to get added-->

<ul>
    {#each items as item, i}
        <div>
            {@render row({data:item})}
        </div>
        {/each}
    </ul>
{@render children?.()}

<style>
      
</style>