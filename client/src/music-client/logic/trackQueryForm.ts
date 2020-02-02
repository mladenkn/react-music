import { Range } from '../shared'

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
    youtubeChannelId: string
    mustHaveEveryTag: string[]
    mustHaveAnyTag: string[]
    yearRange?: Partial<Range<number>>
}

export interface Paging {
    skip: number
    take: number
}

interface TrackQueryFormLogicProps {
    
}

export const useTrackQueryFormLogic = () => {
    
}