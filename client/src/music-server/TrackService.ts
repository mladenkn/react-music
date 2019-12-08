import { TrackData, TrackQueryData, LoadedTracksResponse, YoutubeTrackQuery, UserPermissions } from '../dataModels'
import { TrackRepository } from './TrackRepository'
import YtTrackService, { YTTrackData } from './YtTrackService';
import { groupBy } from 'ramda';

export class TrackService {

    constructor(private repo: TrackRepository, private ytTrackService: YtTrackService){}

    query(q: TrackQueryData): Promise<LoadedTracksResponse> {
        return this.repo.getList(q)
    }

    async queryYoutube(q: YoutubeTrackQuery): Promise<TrackData[]> {
        const fromYt = await this.ytTrackService.query(q)
        return await this.addPropsToYTTracks(fromYt)
    }

    async getRelatedTracks(videoId: string, permissions: UserPermissions){
        if(!permissions.canFetchTrackRecommendations)
            throw new Error('User has no permissons to fetch related tracks')
        const fromYt = await this.ytTrackService.getRelatedTracks(videoId);
        return await this.addPropsToYTTracks(fromYt);
    }

    private async addPropsToYTTracks(fromYt: YTTrackData[]){
        const fromYtIds = fromYt.map(t => t.ytID)
        const trackDatas = await this.repo.getWithIds(fromYtIds)
        const trackDatasById = groupBy(t => t.ytId, trackDatas)
        const r: TrackData[] = fromYt.map(t => {
            const possibleTrackData = trackDatasById[t.ytID] && trackDatasById[t.ytID][0]
            return possibleTrackData || {...t, genres: [], tags: []}
        })
        return r
    }

    async save(tracks: TrackData[], permissions: UserPermissions){
        console.log('TrackService.save begin')
        console.log(tracks)

        if(!permissions.canEditTrackData)
            throw new Error('User has no permissons to edit track')
        
        if(!tracks.some(isTrackValid))
            throw new Error('Track not valid')
    
        await this.repo.save(tracks)
        
        console.log('TrackService.save end')
        return tracks        
    }    
}

const isTrackValid = (track: TrackData) => {
    const conditions = [
        typeof track.ytId === 'string',
        typeof track.title === 'string',
        typeof track.image === 'string',
        typeof track.description === 'string',
        Array.isArray(track.genres),
        Array.isArray(track.tags),
    ]
    return !conditions.includes(false)
}