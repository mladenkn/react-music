import { Track, SaveTrackModel } from "../shared/track";
import { ArrayWithTotalCount } from "../../utils/types";
import { useImmer } from "use-immer";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { TrackQueryForm, TrackQueryFormDataSource, createInitialTrackQueryForm } from "../shared/trackQueryForm";
import { useDebouncedCallback } from 'use-debounce';

export interface Tracklist {
  queryForm: TrackQueryForm
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: string
  currentTrackYoutubeId?: string
  fetchTracksNextPage(): void
  setQueryForm(form: TrackQueryForm): void
  saveTrack(t: SaveTrackModel): Promise<void>
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
    refetchOnChange()
  }, [state.queryForm])

  const [refetchOnChange] = useDebouncedCallback(
    () => fetch(state.queryForm),
    700
  )

  async function fetch(form: TrackQueryForm){
    updateState(draft => {
      draft.fromYouTube = undefined
      draft.fromMusicDb = undefined
    })
    if(form.dataSource === TrackQueryFormDataSource.MusicDb){
      const { data } = await tracksApi.fetchFromMusicDb({ ...form.musicDbParams!, skip: 0, take: pageSize })
      updateState(draft => {
        draft.fromYouTube = undefined
        draft.fromMusicDb = data
      })
    }
    else if(form.dataSource === TrackQueryFormDataSource.YouTube){
      const { data } = await tracksApi.fetchFromYouTube(form.searchQuery!)
      updateState(draft => {
        draft.fromMusicDb = undefined
        draft.fromYouTube = data
      })
    }
  }

  async function fetchTracksNextPage(){
    const skip = state.fromMusicDb!.data.length
    const response = await tracksApi.fetchFromMusicDb({ ...state.queryForm.musicDbParams!, skip, take: pageSize })
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

  function saveTrack(t: SaveTrackModel) {
    const query = { ...state.queryForm.musicDbParams!, skip: 0, take: state.fromMusicDb!.data.length }
    return new Promise<void>((resolve, reject) => {
      tracksApi.save(t, query)
        .then(response => {
          resolve()
          updateState(draft => {
            draft.fromMusicDb = response.data
          })
        })
        .catch(() => {
          reject()
        })
    })    
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