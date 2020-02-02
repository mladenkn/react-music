import { TrackQueryForm, createInitialTrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { tracksApi } from "../apiClient";
import { useImmer } from "use-immer";
import { useEffect } from "react";
import { useRequestLogic } from "./request";
import { HomeSection, State } from "./homeSection.types";

const pageSize = 20;

export const useHomeSection = (): HomeSection => {

    const [state, updateState] = useImmer<State>({
        trackQueryForm: createInitialTrackQueryForm()
    })

    const fetchTracksFromMusicDbLogic = useRequestLogic(tracksApi.fetchFromMusicDb)
    const fetchMoreTracksFromMusicDbLogic = useRequestLogic(tracksApi.fetchFromMusicDb)
    const fetchTracksFromYoutubeLogic = useRequestLogic(tracksApi.fetchFromYT)

    useEffect(() => {
        if(state.trackQueryForm!.dataSource === 'MusicDb'){
            const reqParams = { ...state.trackQueryForm!.fields!, take: pageSize, skip: 0 }
            fetchTracksFromMusicDbLogic.initiate(reqParams)
        }
        else
            fetchTracksFromYoutubeLogic.initiate(state.trackQueryForm.searchQuery!)
    }, [state.trackQueryForm])
    
    const setTrackQueryForm = (trackQueryForm: TrackQueryForm) => {
        updateState(draft => {
            draft.trackQueryForm = trackQueryForm;
        })
    }

    const saveTrack = (t: Track) => tracksApi.save(t)

    return { trackQueryForm: state.trackQueryForm, setTrackQueryForm, saveTrack }
}