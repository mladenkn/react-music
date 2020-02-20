import { Range } from './index'

export enum TrackQueryFormDataSource {
  MusicDb = 'MusicDb',
  YouTube = 'YouTube'
}

export interface TracklistOptions {
  queryForm: {
    dataSource: TrackQueryFormDataSource
    musicDbParams?: MusicDbTrackQueryParams
    searchQuery?: string
  }
  autoRefresh: boolean
  autoPlay: boolean
}

export interface HomeSectionOptions {
  tracklist: TracklistOptions
  tracklistShown: boolean
}

export const createInitialHomeSectionOptions = (): HomeSectionOptions => ({
  tracklist: {
    queryForm: {
      dataSource: TrackQueryFormDataSource.MusicDb,
      musicDbParams: {
        titleContains: '',
        youtubeChannelId: '',
        mustHaveAnyTag: [],
        mustHaveEveryTag: [],
        yearRange: {
        },
        randomize: true
      },
    },
    autoRefresh: true,
    autoPlay: true
  },
  tracklistShown: true
})

export interface MusicDbTrackQueryParams {
  titleContains: string
  youtubeChannelId?: string
  mustHaveEveryTag: string[]
  mustHaveAnyTag: string[]
  yearRange?: Partial<Range<number>>
  randomize: boolean
}