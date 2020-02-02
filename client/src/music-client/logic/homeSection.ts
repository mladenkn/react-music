import { TrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { HomeSection, } from "./homeSection.types";

const pageSize = 20;

export const useHomeSection = (): HomeSection => {    
    
    const setTrackQueryForm = (trackQueryForm: TrackQueryForm) => {
        
    }

    const saveTrack = (t: Track) => tracksApi.save(t)

    return { setTrackQueryForm, saveTrack }
}