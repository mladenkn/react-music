import { TrackQueryForm } from "./trackQueryForm";
import { Track, AsyncOperationStatus } from "../shared";

interface HomeSection {
    fetchTracks(): void
    fetchMoreTracks(): void
    updateTrackQueryForm(form: TrackQueryForm): void
    tracksFetch: {
        data: {
            data: Track[]
            totalCount: number
        }
        status: AsyncOperationStatus
    }
    saveTrack(t: Track): void
    playTrack(trackYoutubeId: string): void
}

export const useHomeSection = () => {

}