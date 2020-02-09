import { AsyncOperationStatus } from "../../utils/types"
import { AxiosResponse } from "axios"
import { useImmer } from 'use-immer'

export interface RequestLogic<TParameters, TData> {
	data?: TData
	error?: any
	status: AsyncOperationStatus | 'NOT_INITIATED'
	initiate(params: TParameters): void
}

interface State {
	lastRequest?: {
		data?: any
		status: AsyncOperationStatus
	}
}

export const useRequestLogic = <TParameters = undefined, TData = undefined>(
	doRequestActual: (params: TParameters) => Promise<AxiosResponse<TData>>)
	: RequestLogic<TParameters, TData> => {
	const [state, updateState] = useImmer<State>({
	})

	const initiate = async (params: TParameters) => {
		updateState(draft => {
			draft.lastRequest = {
				data: undefined,
				status: 'PROCESSING'
			}
		})

		const response = await doRequestActual(params)

		if (response.status >= 200 && response.status < 300)
			updateState(draft => {
				draft.lastRequest!.status = 'PROCESSED'
				draft.lastRequest!.data = response.data as any
			})
		else if (response.status >= 500 && response.status < 500)
			updateState(draft => {
				draft.lastRequest!.status = 'ERROR'
				draft.lastRequest!.data = response.data as any
			})
		else
			throw new Error('Not implemented')
	}

	const didInitiate = !!state.lastRequest

	return didInitiate ? {
		data: state.lastRequest!.data,
		error: undefined,
		status: state.lastRequest!.status,
		initiate
	} : {
			data: undefined,
			error: undefined,
			status: 'NOT_INITIATED',
			initiate
		}
}