import { Range } from './index'
import { ArrayWithTotalCount, IdWithName } from '../../utils/types'
import { Track } from './track'

export enum TrackQueryFormType {
  MusicDbQuery = 'MusicDbQuery',
  YouTubeQuery = 'YouTubeQuery',
}

export interface HomeSectionOptions {
  tracklist: TracklistOptions
  tracklistShown: boolean
}

export type TracklistOptions = {
  query: {
    type: TrackQueryFormType.MusicDbQuery
    musicDbQuery: MusicDbTrackQueryParams
  } | {
    type: TrackQueryFormType.YouTubeQuery
    youTubeQuery: string
  }
  autoRefresh: boolean
  autoPlay: boolean
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
  relatedTracks: number[]
}