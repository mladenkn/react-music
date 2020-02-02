import { TrackQueryForm, MusicDbTrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { AsyncOperationStatus, ArrayWithTotalCount } from "../../utils/types";

class Event {
    readonly at!: Date;
}

class UpdatedTrackQueryForm extends Event {
    readonly form!: TrackQueryForm
}

class InititatedFetchedTracksNextPageFromMusicDb extends Event {
    readonly form!: MusicDbTrackQueryForm
}

class InititatedFetchedTracksFromMusicDb extends Event {
    readonly form!: MusicDbTrackQueryForm
}

class InititatedFetchedTracksFromYouTube extends Event {
    readonly searchQuery!: string
}

class FetchedTracksFromMusicDb extends Event {
    readonly payload!: ArrayWithTotalCount<Track>;
}

class FetchedTracksNextPageFromMusicDb extends Event {
    readonly payload!: ArrayWithTotalCount<Track>;
}

class FetchedTracksFromYouTube extends Event {
    readonly payload!: Track[]
}

class FetchTracksFromMusicDbFailure extends Event {
    readonly error!: {}
}

class FetchTracksNextPageFromMusicDbFailure extends Event {
    readonly error!: {}
}

class FetchTracksFromYouTubeFailure extends Event {
    readonly error!: {}
}

export interface HomeSection {
    trackQueryForm: TrackQueryForm
    tracksFromMusicDb: {
        data?: ArrayWithTotalCount<Track[]>
        status: AsyncOperationStatus
    }
    tracksFromYouTube: {
        data?: Track[]
        status: AsyncOperationStatus
    }
    setTrackQueryForm(trackQueryForm: TrackQueryForm): void
    saveTrack(t: Track): Promise<void>
}