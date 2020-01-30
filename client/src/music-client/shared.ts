export interface Track {
    
}

export interface SaveTrackModel {
    trackYtId: string
    tags: string[]
    year: number
}

export interface Range<T> {
    lowerBound: T
    upperBound: T
}

export enum AsyncOperationStatus {
    NOT_INITIATED='NOT_INITIATED',
    PROCESSING='PROCESSING',
    PROCESSED='PROCESSED',
    ERROR='ERROR',
}