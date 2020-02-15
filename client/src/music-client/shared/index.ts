export interface Track {
  youtubeVideoId: string
  title: string
  image: string
  description: string
  youtubeChannelId: string
  youtubeChannelTitle: string
  year: number
  tags: string[]
}

export interface TrackViewModel extends Track {
  discogsSearchUrl: string
  youtubeVideoUrl: string
  isSelected: boolean
  editableProps: TrackEditableProps
  canFetchRecommendations: boolean
  canEdit: boolean
  canPlay: boolean
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

export interface TrackEditableProps {
  year: number
  tags: string[]
}

export interface Paging {
  skip: number
  take: number
}