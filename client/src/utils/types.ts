export interface ArrayWithTotalCount<T> {
    data: T[]
    totalCount: number
}

export type AsyncOperationStatus = 'PROCESSING' | 'PROCESSED' | 'ERROR'