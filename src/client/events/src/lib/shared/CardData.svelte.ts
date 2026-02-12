import { SvelteDate } from "svelte/reactivity";
import type { IiterDisplay } from "./IteratableItem";

//# means private... imagine why do a keyword L. Ts.
export class CardData{

    _current: number =0;

    eventName: string = $state('');
    covers: string[] = $state([] as string[]);
    currentCover: string = $derived(this.Cover(this.covers));
    dateStart: Date = new SvelteDate();
    dateEnd: Date = new SvelteDate();
    eventType: string | null =  $state('');
    features: string[] | null = $state([]);

    constructor(
        dateStart: Date,
        dateEnd: Date,
        covers: string[],
        features: string[] | null = null, 
        eventType: string | null = null,
        eventName: string)
        {

        this.dateStart.setTime(dateStart.getTime())
        this.dateEnd.setTime(dateEnd.getTime())
        this.covers = covers;
        this.eventType = eventType;
        this.features = features;
        this.eventName = eventName;
    }
    
    Cover(available: string[]){
        return available[this._current++ > this.covers.length ?
             this._current = 0 : this._current ++]
    }
}

export interface cardItem extends IiterDisplay {
    data: CardData,
    width: string | null,
    height: string | null,
}