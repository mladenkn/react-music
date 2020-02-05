import { createEvent } from "../../utils/events";
import { TrackQueryForm } from "./trackQueryForm";
import { ArrayWithTotalCount } from "../../utils/types";
import { Track } from "../shared";

export const updatedQueryTrackForm = createEvent('updatedQueryTrackForm', (form: TrackQueryForm) => form)

export const initiatedTracksFetch = createEvent('initiatedNextPageTracksFetchFromMusicDb', (form: TrackQueryForm) => form)
export const fetchedTracks = createEvent('fetchedTracks', (tracks: ArrayWithTotalCount<Track>) => tracks)
export const fetchTracksFailed = createEvent('fetchTracksFailed', () => {})