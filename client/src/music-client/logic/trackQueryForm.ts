import { Range } from '../shared'

export interface TrackQueryForm {
    dataSource: 'MusicDb' | 'YouTube'
    fields: MusicDbTrackQueryForm | string
}

export interface MusicDbTrackQueryForm {
    titleContains: string
    youtubeChannelId: string
    mustHaveEveryTag: string[]
    mustHaveAnyTag: string[]
    yearRange: Range<number>
    skip: number
    take: number    
}

interface TrackQueryFormLogicProps {
    
}

export const useTrackQueryFormLogic = () => {
    
}