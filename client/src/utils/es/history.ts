import { Event, EventCreator } from './events';
import { useState } from 'react';

export interface History {
    save(event: Event): void
    whereType<TEventArgs extends any[], TEventPayload>(eventCreators: EventCreator<TEventArgs, TEventPayload>): Event<TEventPayload>[]
    latestWhereType<TEventArgs extends any[], TEventPayload>(eventCreators: EventCreator<TEventArgs, TEventPayload>): Event<TEventPayload> | undefined
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

    const whereType = <TEventArgs extends any[], TEventPayload>(eventCreator: EventCreator<TEventArgs, TEventPayload>) => {
      const eventsOfType = state[eventCreator.type]
      if(!eventsOfType)
        return []
      return eventsOfType.filter(e => e.type === eventCreator.type) as Event<TEventPayload>[]
    }

    const whereTypeSingle = <TEventArgs extends any[], TEventPayload>(eventCreator: EventCreator<TEventArgs, TEventPayload>) => {
      const eventsOfType = state[eventCreator.type]
      return eventsOfType && eventsOfType.find(e => e.type === eventCreator.type) as Event<TEventPayload> | undefined
    }

    return { save, whereType, latestWhereType: whereTypeSingle }
}