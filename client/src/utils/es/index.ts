import { useImmer } from "use-immer"

export interface EventHistory {
	save(event: {}): void
	lastOf<TType>(type: {}): TType
	lastOfAnyType(types: {}[]): {}[]
}

interface EventHistoryState {
	[key: string]: {}[]
}

export const useEventHistory = (): EventHistory => {

	const [state, updateState] = useImmer<EventHistoryState>({})

	const save = (event: {}) => {
		const eventType = typeof event
		updateState(draft => {
			if (!draft[eventType])
				state[eventType] = [event]
			else
				state[eventType].push(event)
		})
	}

	const lastOf = <TType>(type: {}): TType => {
		throw new Error('Not implemented')
	}

	const lastOfAnyType = (types: {}[]): {}[] => {
		throw new Error('Not implemented')
	}

	return { save, lastOf, lastOfAnyType }
}