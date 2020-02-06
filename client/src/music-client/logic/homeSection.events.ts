import { createEvent } from "../../utils/es/events";
import { TrackQueryForm, MusicDbTrackQueryForm } from "./trackQueryForm";
import { ArrayWithTotalCount, Identifable, BelongsToRequest } from "../../utils/types";
import { Track } from "../shared";

export const updatedQueryTrackForm = createEvent('updatedQueryTrackForm', (form: TrackQueryForm) => form)

export const initiatedTracksFetch = createEvent(
    'initiatedTracksFetch', 
    (req: Identifable<TrackQueryForm>) => req
)
export const fetchTracksFailed = createEvent('fetchTracksFailed', (requestId: number) => ({ requestId }))

export const fetchedTracksFromMusicDb = createEvent(
    'fetchedTracksFromMusicDb', 
    (response: BelongsToRequest<ArrayWithTotalCount<Track>>) => response
)
export const fetchedTracksFromYouTube = createEvent('fetchedTracksFromYoutube', (response: BelongsToRequest<Track[]>) => response)

export const initiatedTracksNextPageFetch = createEvent(
    'initiatedTracksNextPageFetch', 
    (req: Identifable<MusicDbTrackQueryForm>) => req
)
export const fetchedTracksNextPage = createEvent(
    'fetchedTracksNextPage', 
    (response: BelongsToRequest<ArrayWithTotalCount<Track>>) => response
)
export const fetchTracksNextPageFailed = createEvent('fetchTracksNextPageFailed', (requestId: number) => ({ requestId }))