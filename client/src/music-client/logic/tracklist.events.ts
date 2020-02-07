import { createEvent } from "../../utils/es/events"
import { TrackQueryForm, MusicDbTrackQueryForm } from "./trackQueryForm"
import { ArrayWithTotalCount, Identifable } from "../../utils/types"
import { Track } from "../shared"

export const updatedQueryTrackForm = createEvent(
  "updatedQueryTrackForm",
  (form: TrackQueryForm) => form
)

export const initiatedTracksFetch = createEvent(
  "initiatedTracksFetch",
  (req: Identifable<TrackQueryForm>) => req
)
export const fetchTracksFailed = createEvent(
  "fetchTracksFailed",
  (requestId: number) => ({ requestId })
)

export const fetchedTracksFromMusicDb = createEvent(
  "fetchedTracksFromMusicDb",
  (response: { requestId: number; data: ArrayWithTotalCount<Track> }) =>
    response
)
export const fetchedTracksFromYouTube = createEvent(
  "fetchedTracksFromYoutube",
  (response: { requestId: number; data: Track[] }) => response
)

export const initiatedTracksNextPageFetch = createEvent(
  "initiatedTracksNextPageFetch",
  (req: Identifable<MusicDbTrackQueryForm>) => req
)
export const fetchedTracksNextPage = createEvent(
  "fetchedTracksNextPage",
  (response: {
    initialRequestId: number
    id: number
    data: ArrayWithTotalCount<Track>
  }) => response
)
export const fetchTracksNextPageFailed = createEvent(
  "fetchTracksNextPageFailed",
  (a: { initialRequestId: number; requestId: number }) => a
)
