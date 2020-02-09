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


export interface TrackQueryForm {
  dataSource: 'MusicDb' | 'YouTube'
  fields?: MusicDbTrackQueryForm
  searchQuery?: string
}

export const createInitialTrackQueryForm = (): TrackQueryForm => ({
  dataSource: 'MusicDb',
  fields: {
      titleContains: '',
      youtubeChannelId: '',
      mustHaveAnyTag: [],
      mustHaveEveryTag: [],
      yearRange: {
          
      }
  }
})

export interface MusicDbTrackQueryForm {
  titleContains: string
  youtubeChannelId?: string
  mustHaveEveryTag: string[]
  mustHaveAnyTag: string[]
  yearRange?: Partial<Range<number>>
}

export interface Paging {
  skip: number
  take: number
}