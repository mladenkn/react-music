export interface ArrayWithTotalCount<T> {
  data: T[]
  totalCount: number
}

export type AsyncOperationStatus = 'PROCESSING' | 'PROCESSED' | 'ERROR'

export interface Identifable<TData> {
  id: number
  data: TData
}

export interface BelongsToRequest<TData = {}> {
  requestId: number
  data: TData
}

export interface IdWithName {
  id: string
  name: string
}