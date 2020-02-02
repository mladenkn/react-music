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