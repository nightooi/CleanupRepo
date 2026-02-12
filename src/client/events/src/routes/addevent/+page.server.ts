import { fail } from '@sveltejs/kit';
import type { Actions } from './$types';

function parseDate(value: FormDataEntryValue | null) {
    if (typeof value !== 'string' || !value) return null;
    // expecting yyyy-mm-dd from <input type="date">
    const d = new Date(value);
    return Number.isNaN(d.getTime()) ? null : d;
}

function parseLines(value: FormDataEntryValue | null) {
    if (typeof value !== 'string') return [];
    return value
        .split(/\r?\n|,/g)
        .map((s) => s.trim())
        .filter(Boolean);
}

function parseOptionalString(value: FormDataEntryValue | null) {
    if (typeof value !== 'string') return null;
    const v = value.trim();
    return v.length ? v : null;
}

export const actions: Actions = {
    create: async ({ request }) => {
        const fd = await request.formData();

        const eventName = parseOptionalString(fd.get('eventName'));
        const eventType = parseOptionalString(fd.get('eventType'));
        const dateStart = parseDate(fd.get('dateStart'));
        const dateEnd = parseDate(fd.get('dateEnd'));

        const covers = parseLines(fd.get('covers'));
        const features = parseLines(fd.get('features'));
        const width = parseOptionalString(fd.get('width'));
        const height = parseOptionalString(fd.get('height'));

        const errors: Record<string, string> = {};

        if (!eventName) errors.eventName = 'Event name is required.';
        if (!dateStart) errors.dateStart = 'Start date is required.';
        if (!dateEnd) errors.dateEnd = 'End date is required.';
        if (dateStart && dateEnd && dateEnd < dateStart) errors.dateEnd = 'End date must be after start date.';

        if (!covers.length) errors.covers = 'At least one cover URL/path is required.';

        const values = {
            eventName: eventName ?? '',
            eventType: eventType ?? '',
            dateStart: typeof fd.get('dateStart') === 'string' ? (fd.get('dateStart') as string) : '',
            dateEnd: typeof fd.get('dateEnd') === 'string' ? (fd.get('dateEnd') as string) : '',
            covers: typeof fd.get('covers') === 'string' ? (fd.get('covers') as string) : '',
            features: typeof fd.get('features') === 'string' ? (fd.get('features') as string) : '',
            width: width ?? '',
            height: height ?? ''
        };
        if (Object.keys(errors).length) {
            return fail(400, {
                errors, values
            });
        }

        const payload = {
            eventName: eventName!,
            dateStart: dateStart!.toISOString(),
            dateEnd: dateEnd!.toISOString(),
            covers,
            eventType,
            features: features.length ? features : []
        };

        const res = await fetch('http://localhost:5133/events/Events', {
            method: 'POST',
            headers: { 'content-type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            const text = await res.text().catch(() => '');
            return fail(res.status, {
                errors: { backend: `Backend error ${res.status}${text ? `: ${text}` : ''}` },
                values
            });
        }
        console.log(payload)

        let dto: unknown = payload;

        try {
            dto = await res.json();
        } catch {
            // ignore if no JSON body
        }
        // Return a plain DTO
        
        console.log(`DTO$`)
        console.log(dto)
        return { dto };
    }
};