import { Event, EventCreator } from './events';
import { useState } from 'react';

export interface History {
    save(event: Event): void
    whereType<TEventArgs extends any[], TEventPayload>(eventCreators: EventCreator<TEventArgs, TEventPayload>): Event<TEventPayload>[]
    whereTypeSingle<TEventArgs extends any[], TEventPayload>(eventCreators: EventCreator<TEventArgs, TEventPayload>): Event<TEventPayload> | undefined
}

interface EventHistoryState {
    [key: string]: Event[] | undefined
}

export const useHistory = (): History => {
    const [state, updateState] = useState<EventHistoryState>({})

    const save = (event: Event) => {
        const eventsOfType = [event, ...state[event.type]!]
        updateState({ ...state, [event.type]: eventsOfType })
    }

    throw new Error('Not implemented.')
}