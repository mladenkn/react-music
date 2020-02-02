import { TrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { AsyncOperationStatus, ArrayWithTotalCount } from "../../utils/types";

// export interface FetchedTracksFromMusicDbEvent {
//     type: 'FetchedTracksFromMusicDbEvent',
//     data: ArrayWithTotalCount<Track>
// }

// export interface FetchedTracksNextPageFromMusicDbEvent {
//     type: 'FetchedTracksNextPageFromMusicDbEvent',
//     data: ArrayWithTotalCount<Track>
// }

// export interface FetchedTracksFromYouTubeEvent {
//     type: 'FetchedTracksFromYouTubeEvent',
//     data: Track[]
// }

// type TracksFetchEvent = FetchedTracksFromMusicDbEvent | FetchedTracksNextPageFromMusicDbEvent | FetchedTracksFromYouTubeEvent

export interface HomeSection {
    trackQueryForm: TrackQueryForm
    tracksFromMusicDb: {
        data: ArrayWithTotalCount<Track[]>
        status: AsyncOperationStatus
    }
    tracksFromYouTube: {
        data: Track[]
        status: AsyncOperationStatus
    }
    setTrackQueryForm(trackQueryForm: TrackQueryForm): void
    saveTrack(t: Track): Promise<void>
}

export interface State {
    trackQueryForm: TrackQueryForm
    // lastTracksFetchEvent: TracksFetchEvent
}