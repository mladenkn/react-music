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
  const [state, setState] = useState<EventHistoryState>({})
  
  console.log(state)

  const save = (event: Event) => {
    console.log(state)
    const curEventsOfType = state[event.type] || []
    const eventsOfType = [event, ...curEventsOfType]
    const newState = { ...state, [event.type]: eventsOfType }
    setState(newState)
  }
 
  const whereType = <TEventArgs extends any[], TEventPayload> (eventCreator: EventCreator<TEventArgs, TEventPayload>) => {
    const eventsOfType = state[eventCreator.type]
    if (!eventsOfType)
      return []
    return eventsOfType.filter(e => e.type === eventCreator.type) as Event<TEventPayload>[]
  }

  const whereTypeSingle = <TEventArgs extends any[], TEventPayload>(eventCreator: EventCreator<TEventArgs, TEventPayload>) => {
    const eventsOfType = state[eventCreator.type]
    return eventsOfType?.find(e => e.type === eventCreator.type) as Event<TEventPayload> | undefined
  }

  return { save, whereType, latestWhereType: whereTypeSingle }
}