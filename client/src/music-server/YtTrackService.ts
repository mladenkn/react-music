import axios from 'axios'
import { YoutubeTrackQuery } from '../dataModels';

export type Duration = {
    minutes: number
    seconds: number
}

export interface YTTrackData {
    ytID: string
    title: string
    image: string
    description: string
    channel: {
        id: string,
        title: string
    }
    //duration: Duration
}

const parseDuration = (durationStr: string) => {
    const getBetween = (from: string, to: string) => 
        parseInt(durationStr.substring(durationStr.indexOf(from) + from.length , durationStr.indexOf(to)))
    const minutes = getBetween('PT', 'M')
    const seconds = getBetween('M', 'S')
    return { minutes, seconds }
}

export default class {

    private key = 'AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM'

    getWithIds(ids: string[]): Promise<YTTrackData[]>{
        return axios.get('https://www.googleapis.com/youtube/v3/videos', {params: {
            part: 'snippet, contentDetails', key: this.key, id: ids.join(',')
        }})
        .then(r =>
            r.data.items.map((t: any) => {
                const t_ = mapTrack(t);
                return { ...t_, ytID: t.id };
            })
        ) 
    }

    async query(q: YoutubeTrackQuery): Promise<YTTrackData[]>{

        let channelId: string | undefined
        if(q.channelTitle){
            channelId = await this.fetchChannelId(q.channelTitle)
            // handle channel not found
        }

        const tracksResult = await axios.get('https://www.googleapis.com/youtube/v3/search', {params: {
            part: 'snippet',
            channelId, 
            q: q.searchQuery, 
            maxResults: q.maxResults, 
            key: this.key, 
            type: 'video'
        }})
        return tracksResult.data.items.map(mapTrack)
    }

    async fetchChannelId(channelTitle: string){
        const r = await axios.get('https://www.googleapis.com/youtube/v3/channels', {params: {
            part: 'id', forUsername: channelTitle, key: this.key
        }})
        return r.data.items[0].id
    }

    getRelatedTracks (videoId: string): Promise<YTTrackData[]>{
        return axios.get('https://www.googleapis.com/youtube/v3/search', {params: {
            part: 'snippet', relatedToVideoId: videoId, key: this.key, type: 'video'
        }})
        .then(r => r.data.items.map(mapTrack))            
    }
}

const mapTrack = (t: any): YTTrackData => ({
    ytID: t.id.videoId as string,
    title: t.snippet.title as string,
    image: t.snippet.thumbnails.medium.url as string,
    description: t.snippet.description as string,
    channel: {
        id: t.snippet.channelId,
        title: t.snippet.channelTitle
    },
    //duration: parseDuration(t.contentDetails.duration)
})