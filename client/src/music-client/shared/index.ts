export interface Paging {
  skip: number
  take: number
}

export interface Range<T> {
  lowerBound: T
  upperBound: T
}