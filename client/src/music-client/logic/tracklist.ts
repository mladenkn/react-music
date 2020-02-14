import { TrackQueryForm, Track, createInitialTrackQueryForm } from "../shared";
import { ArrayWithTotalCount } from "../../utils/types";
import { useImmer } from "use-immer";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";

export interface Tracklist {
  queryForm: TrackQueryForm
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: string
  currentTrackYoutubeId?: string
  fetchTracksNextPage(): void
  setQueryForm(form: TrackQueryForm): void
  saveTrack(t: Track): Promise<void>
  onTrackClick(trackYoutubeId: string): void
  setCurrentTrack(trackYoutubeId: string): void
}

interface State {
  queryForm: TrackQueryForm
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: string
  currentTrackYoutubeId?: string
}

const pageSize = 30;

export const useTracklistLogic = (): Tracklist => {

  const [state, updateState] = useImmer<State>({
    queryForm: createInitialTrackQueryForm(),
  })

  useEffect(() => {
    fetch(state.queryForm)
  }, [state.queryForm])

  async function fetch(form: TrackQueryForm){
    if(form.fields){
      const { data } = await tracksApi.fetchFromMusicDb({ ...form.fields, skip: 0, take: pageSize })
      updateState(draft => {
        draft.fromYouTube = undefined
        draft.fromMusicDb = data
      })
    }
    else if(form.searchQuery){
      const { data } = await tracksApi.fetchFromYouTube(form.searchQuery)
      updateState(draft => {
        draft.fromMusicDb = undefined
        draft.fromYouTube = data
      })
    }
  }

  async function fetchTracksNextPage(){
    const skip = state.fromMusicDb!.data.length
    const response = await tracksApi.fetchFromMusicDb({ ...state.queryForm.fields!, skip, take: pageSize })
    updateState(draft => {
      draft.fromMusicDb!.totalCount = response.data.totalCount
      draft.fromMusicDb!.data = [ ...draft.fromMusicDb!.data, ...response.data.data ]
    })
  }
  
  function setQueryForm(form: TrackQueryForm) {
    updateState(draft => {
      draft.queryForm = form
    })
  }

  function saveTrack(t: Track) {
    return tracksApi.save(t)
  }
  
  function onTrackClick(trackYoutubeId: string) {
    updateState(draft => {
      draft.selectedTrackId = trackYoutubeId
    })
  }

  function setCurrentTrack(trackYoutubeId: string){
    updateState(draft => {
      draft.currentTrackYoutubeId = trackYoutubeId
    })
  }

  return { ...state, fetchTracksNextPage, setQueryForm, saveTrack, onTrackClick, setCurrentTrack }
}