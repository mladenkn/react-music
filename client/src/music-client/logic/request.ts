import { AsyncOperationStatus } from "../shared"

export interface RequestLogic<TParameters, TData> {
    data?: TData
    errorMessage?: string
    status: AsyncOperationStatus
    initiate(params: TParameters): void
}

export const useRequestLogic = <TParameters = undefined, TData = undefined> (url: string): RequestLogic<TParameters, TData> => {
    throw new Error('Not implemented.')
}