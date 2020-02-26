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
  selectedTrackYoutubeId?: string
  currentTrackYoutubeId?: string
}

export interface HomeSectionPropsFromApi {
  options: HomeSectionOptions
  selectedTrackYoutubeId?: string
  currentTrackYoutubeId?: string
  tracksFromMusicDb?: ArrayWithTotalCount<Track>
  tracksFromYouTube?: Track[]
  youTubeChannels: IdWithName[]
  tags: string[]
}

export const createInitialHomeSectionOptions = (): HomeSectionOptions => ({
  tracklist: {
    queryForm: {
      dataSource: TrackQueryFormDataSource.MusicDb,
      musicDbQuery: {
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