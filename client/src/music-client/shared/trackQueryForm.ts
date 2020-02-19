import { Range } from './index'

export enum TrackQueryFormDataSource {
  MusicDb = 'MusicDb',
  YouTube = 'YouTube'
}

export interface HomeSectionOptions {
  dataSource: TrackQueryFormDataSource
  musicDbParams?: MusicDbTrackQueryParams
  searchQuery?: string
  autoRefresh: boolean
  autoPlay: boolean
}

export const createInitialHomeSectionOptions = (): HomeSectionOptions => ({
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
  autoRefresh: true,
  autoPlay: true
})

export interface MusicDbTrackQueryParams {
  titleContains: string
  youtubeChannelId?: string
  mustHaveEveryTag: string[]
  mustHaveAnyTag: string[]
  yearRange?: Partial<Range<number>>
  randomize: boolean
}