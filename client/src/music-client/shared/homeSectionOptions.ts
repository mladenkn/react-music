import { Range } from './index'
import { ArrayWithTotalCount, IdWithName } from '../../utils/types'
import { Track } from './track'

export enum TrackQueryFormDataSource {
  MusicDb = 'MusicDb',
  YouTube = 'YouTube'
}

export interface TracklistOptions {
  queryForm: {
    dataSource: TrackQueryFormDataSource
    musicDbQuery?: MusicDbTrackQueryParams
    youTubeQuery?: string
  }
  autoRefresh: boolean
  autoPlay: boolean
}

export interface HomeSectionOptions {
  tracklist: TracklistOptions
  tracklistShown: boolean
}

export interface HomeSectionPersistableState {
  options: HomeSectionOptions
  selectedTrackId?: number
  currentTrackId?: number
}

export interface HomeSectionPropsFromApi {
  options: HomeSectionOptions
  selectedTrackId?: string
  currentTrackId?: string
  tracksFromMusicDb?: ArrayWithTotalCount<Track>
  tracksFromYouTube?: Track[]
  youTubeChannels: IdWithName[]
  tags: string[]
}

export interface MusicDbTrackQueryParams {
  titleContains: string
  supportedYouTubeChannelsIds: string[]
  mustHaveEveryTag: string[]
  mustHaveAnyTag: string[]
  yearRange?: Partial<Range<number>>
  randomize: boolean
}