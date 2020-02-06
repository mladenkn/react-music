import { createEvent } from "../../utils/es/events";
import { TrackQueryForm, MusicDbTrackQueryForm } from "./trackQueryForm";
import { ArrayWithTotalCount } from "../../utils/types";
import { Track } from "../shared";

export const updatedQueryTrackForm = createEvent('updatedQueryTrackForm', (form: TrackQueryForm) => form)

export const initiatedTracksFetch = createEvent(
    'initiatedNextPageTracksFetchFromMusicDb', 
    (form: TrackQueryForm) => form
)
export const fetchTracksFailed = createEvent('fetchTracksFailed', () => ({}))

export const fetchedTracksFromMusicDb = createEvent(
    'fetchedTracksFromMusicDb', 
    (tracks: ArrayWithTotalCount<Track>) => tracks
)
export const fetchedTracksFromYouTube = createEvent('fetchedTracksFromYoutube', (tracks: Track[]) => tracks)

export const initiatedTracksNextPageFetch = createEvent(
    'initiatedTracksNextPageFetch', 
    (form: MusicDbTrackQueryForm) => form
)
export const fetchedTracksNextPage = createEvent(
    'fetchedTracksNextPage', 
    (tracks: ArrayWithTotalCount<Track>) => tracks
)
export const fetchTracksNextPageFailed = createEvent('fetchTracksNextPageFailed', () => ({}))