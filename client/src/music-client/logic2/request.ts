export interface RequestState<TData = undefined> {
    id: number
    data?: TData
    errorMessage?: string
    status: 'PROCESSING' | 'PROCESSED' | 'ERROR'
}

export interface RequestLogic<TParameters> {
    lastOne: RequestState,
    initiate(params: TParameters): void
}

export const useRequestLogic = <TParameters = undefined> (url: string): RequestLogic<TParameters> => {
    
}