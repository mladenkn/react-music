import { Range } from './index'

export enum TrackQueryFormDataSource {
  MusicDb = 'MusicDb',
  YouTube = 'YouTube'
}

export interface TrackQueryForm {
  dataSource: TrackQueryFormDataSource
  musicDbParams?: MusicDbTrackQueryParamas
  searchQuery?: string
  autoRefresh: boolean
}

export const createInitialTrackQueryForm = (): TrackQueryForm => ({
  dataSource: TrackQueryFormDataSource.MusicDb,
  musicDbParams: {
    titleContains: '',
    youtubeChannelId: '',
    mustHaveAnyTag: [],
    mustHaveEveryTag: [],
    yearRange: {        
    }
  },
  autoRefresh: true,
})

export interface MusicDbTrackQueryParamas {
  titleContains: string
  youtubeChannelId?: string
  mustHaveEveryTag: string[]
  mustHaveAnyTag: string[]
  yearRange?: Partial<Range<number>>
}