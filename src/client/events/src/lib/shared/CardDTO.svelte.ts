
import { writable } from "svelte/store";
import {CardData, type cardItem} from "./CardData.svelte"

export const events = writable<CardPostDTO[]>([]);

export type CardPostDTO = {
    id: number;
    data: {
        dateStart: string;
        dateEnd: string;
        covers: string[];
        features: string[] | null;
        eventType: string | null;
        eventName: string;
    };
    width: string | null;
    height: string | null;
};

export type EventData = {
    id: string;
    eventName: string;
    eventStart: string;
    eventEnd: string;
    covers: string[] | null;
    eventType: string | null;
    features: string[] | null;
}

export function toDTO(x: cardItem): CardPostDTO {

    const res = {
        id: x.id,
        width: x.width,
        height: x.height,
        data: {
            dateStart: x.data.dateStart.toISOString(),
            dateEnd: x.data.dateEnd.toISOString(),
            covers: [...x.data.covers],
            features: x.data.features ? [...x.data.features] : null,
            eventType: x.data.eventType,
            eventName: x.data.eventName
        }
    };
    return res;
}

export function fromDTO(dto: CardPostDTO): cardItem {
    return {
        id: dto.id,
        width: dto.width,
        height: dto.height,
        data: new CardData(
            new Date(dto.data.dateStart),
            new Date(dto.data.dateEnd),
            dto.data.covers,
            dto.data.features,
            dto.data.eventType,
            dto.data.eventName
        )
    };
}
