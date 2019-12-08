import { TrackDataForm } from "./music-client/view/TrackQueryInteractiveForm/rootLogic"

export interface TrackData {
    ytId: string
    title: string
    image: string
    description: string
    year?: number
    genres: string[]
    tags: string[]
    channel: {
        id: string
        title: string
    }
    // thumbnails
}

// export interface TrackDataEditableProps {
//     year?: number
//     genres?: string[]
//     tags?: string[]
// }
 
export interface TrackQueryData extends TrackDataForm {
    skip: number
    take: number
} 

export interface LoadedTracksResponse {
    data: TrackData[]
    totalCount: number
    thereIsMore: boolean
}

export interface YoutubeTrackQuery  {
    searchQuery: string | undefined
    channelTitle: string | undefined
    maxResults: number
}

export type WithUserKey = { userKey: string }

export type UserPermissions = {
    canEditTrackData: boolean
    canFetchTrackRecommendations: boolean
}

export type WithUserPermissions = { permissions: UserPermissions }