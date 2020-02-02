import { TrackQueryForm, createInitialTrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { tracksApi } from "../apiClient";
import { useImmer } from "use-immer";

interface HomeSection {
    trackQueryForm: TrackQueryForm
    setTrackQueryForm(trackQueryForm: TrackQueryForm): void
    saveTrack(t: Track): void
}

interface State {
    trackQueryForm: TrackQueryForm
}

const pageSize = 20;

export const useHomeSection = (): HomeSection => {

    const [state, updateState] = useImmer<State>({
        trackQueryForm: createInitialTrackQueryForm()
    })
    
    const setTrackQueryForm = (trackQueryForm: TrackQueryForm) => {
        if(state.trackQueryForm!.dataSource === 'MusicDb'){
            const reqParams = { ...state.trackQueryForm!.fields!, take: pageSize, skip: 0 }
            return tracksApi.fetchFromMusicDb(reqParams)
                .then(response => ({ fromMusicDbSource: response.data }))
        }
        else 
            return tracksApi.fetchFromYT(state.trackQueryForm.searchQuery!)
                .then(response => ({ fromYt: response.data }))
    }

    const saveTrack = (t: Track) => tracksApi.save(t)

    return { trackQueryForm: state.trackQueryForm, setTrackQueryForm, saveTrack }
}