import { AsyncOperationStatus } from "../../utils/types"
import { AxiosResponse } from "axios"
import { useImmer } from 'use-immer'

export interface RequestLogic<TParameters, TData> {
    data?: TData
    errorMessage?: string
    status: AsyncOperationStatus | 'NOT_INITIATED'
    initiate(params: TParameters): void
}

interface State {
    nextRequestId: number,
    lastRequest?: {
        data?: any
        status: AsyncOperationStatus
    }
}

export const useRequestLogic = <TParameters = undefined, TData = undefined> (
    doRequestActual: (params: TParameters) => Promise<AxiosResponse<TData>>)
    : RequestLogic<TParameters, TData> => 
{        
    const [state, updateState] = useImmer<State>({
        nextRequestId: 1,
    })

    const initiate = async (params: TParameters) => {
        updateState(draft => {            
            draft.lastRequest = {
                data: undefined,
                status: 'PROCESSING'
            }
            draft.nextRequestId++
        })   
        
        const response = await doRequestActual(params)

        if(response.status >= 200  &&  response.status < 300)
            updateState(draft => {
                draft.lastRequest!.status = 'PROCESSED'
                draft.lastRequest!.data = response.data as any
            })
        else if(response.status >= 500  &&  response.status < 500)
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
        status: state.lastRequest!.status,
        errorMessage: undefined,
        initiate
    } : {
        data: undefined,
        errorMessage: state.lastRequest!.data!.errorMessage,
        status: 'NOT_INITIATED',
        initiate
    }
}