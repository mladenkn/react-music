import { Track, SaveTrackModel, mapToTrackViewModel, TrackViewModel } from "../shared/track";
import { ArrayWithTotalCount } from "../../utils/types";
import { useImmer } from "use-immer";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { HomeSectionOptions, TrackQueryFormDataSource, createInitialHomeSectionOptions } from "../shared/homeSectionOptions";
import { useDebouncedCallback } from 'use-debounce';

export interface Tracklist {
  options: HomeSectionOptions
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: string
  tracks: TrackViewModel[] | undefined
  tracksTotalCount?: number
  fetchTracksNextPage(): void
  setQueryForm(form: HomeSectionOptions): void
  saveTrack(t: SaveTrackModel): Promise<void>
  onTrackClick(trackYoutubeId: string): void
  fetchTracks(): void
}

interface State {
  options: HomeSectionOptions
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: string
}

const pageSize = 30;

export const useTracklistLogic = (): Tracklist => {

  const [state, updateState] = useImmer<State>({
    options: createInitialHomeSectionOptions(),
  })

  useEffect(() => {
    if(state.options.autoRefresh)
      refetchOnChange()
  }, [state.options.tracksQueryForm])

  const [refetchOnChange] = useDebouncedCallback(
    () => fetch(),
    700
  )

  async function fetch(){
    const { tracksQueryForm: form } = state.options
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
    const response = await tracksApi.fetchFromMusicDb({ ...state.options.tracksQueryForm.musicDbParams!, skip, take: pageSize })
    updateState(draft => {
      draft.fromMusicDb!.totalCount = response.data.totalCount
      draft.fromMusicDb!.data = [ ...draft.fromMusicDb!.data, ...response.data.data ]
    })
  }
  
  function setQueryForm(form: HomeSectionOptions) {
    updateState(draft => {
      draft.options = form
    })
  }

  function saveTrack(track: SaveTrackModel) { 
    if(state.options.tracksQueryForm.dataSource === TrackQueryFormDataSource.MusicDb){
      const query = { ...state.options.tracksQueryForm.musicDbParams!, skip: 0, take: state.fromMusicDb!.data.length }
      return new Promise<void>((resolve, reject) => {
        tracksApi.save(track, query)
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
    else {
      return new Promise<void>((resolve, reject) => {
        tracksApi.save(track)
          .then(() => {
            resolve()
            updateState(draft => {
              const track_ = draft.fromYouTube!.find(t => t.youtubeVideoId === track.trackYtId)!
              track_.tags = track.tags
              track_.year = track.year
            })
          })
          .catch(() => {
            reject()
          })
      })
    }
  }
  
  function onTrackClick(trackYoutubeId: string) {
    updateState(draft => {
      draft.selectedTrackId = trackYoutubeId
    })
  }

  let tracks: TrackViewModel[] | undefined
  if(state.fromMusicDb || state.fromYouTube){
    const beforeMap = state.fromMusicDb ? state.fromMusicDb.data : state.fromYouTube!
    tracks = beforeMap.map(track => mapToTrackViewModel(track, state.selectedTrackId))
  }

  const tracksTotalCount = state.fromMusicDb && state.fromMusicDb.totalCount

  return { 
    ...state, 
    tracks,
    tracksTotalCount,    
    fetchTracks: fetch, 
    fetchTracksNextPage, 
    setQueryForm, 
    saveTrack, 
    onTrackClick,
  }
}