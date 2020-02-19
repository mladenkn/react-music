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

  const { tracksQueryForm } = state.options

  async function fetch(){
    updateState(draft => {
      draft.fromYouTube = undefined
      draft.fromMusicDb = undefined
    })
    if(tracksQueryForm.dataSource === TrackQueryFormDataSource.MusicDb){
      const { data } = await tracksApi.fetchFromMusicDb({ ...tracksQueryForm.musicDbParams!, skip: 0, take: pageSize })
      updateState(draft => {
        draft.fromYouTube = undefined
        draft.fromMusicDb = data
      })
    }
    else if(tracksQueryForm.dataSource === TrackQueryFormDataSource.YouTube){
      const { data } = await tracksApi.fetchFromYouTube(tracksQueryForm.searchQuery!)
      updateState(draft => {
        draft.fromMusicDb = undefined
        draft.fromYouTube = data
      })
    }
  }

  async function fetchTracksNextPage(){
    const skip = state.fromMusicDb!.data.length
    const response = await tracksApi.fetchFromMusicDb({ ...tracksQueryForm.musicDbParams!, skip, take: pageSize })
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

  function doesPassCurrentFilter(track: { tags: string[], year: number }){
    if(tracksQueryForm.dataSource === TrackQueryFormDataSource.YouTube)
      return true
    else {
      const filter = tracksQueryForm.musicDbParams!
      if(filter.mustHaveAnyTag && filter.mustHaveAnyTag.length > 0){
        const hasAny = track.tags.some(t => filter.mustHaveAnyTag.includes(t))
        if(!hasAny)
          return false
      }
      if(filter.mustHaveEveryTag && filter.mustHaveEveryTag.length > 0){
        debugger
        const hasAll = track.tags.every(t => filter.mustHaveEveryTag.includes(t))
        if(!hasAll)
          return false
      }
      if(filter.yearRange){
        if(filter.yearRange.lowerBound)
          if(track.year < filter.yearRange.lowerBound)
            return false
        if(filter.yearRange.upperBound)
          if(track.year > filter.yearRange.upperBound)
            return false
      }
      return true
    }
  }

  function saveTrack(editedTrackFromUser: SaveTrackModel) {
    return new Promise<void>((resolve, reject) => {
      tracksApi.save(editedTrackFromUser)
        .then(() => {
          resolve()
          updateState(draft => {
            if(doesPassCurrentFilter(editedTrackFromUser)){
              const track_ = (draft.fromYouTube || draft.fromMusicDb!.data).find(t => t.youtubeVideoId === editedTrackFromUser.trackYtId)!
              track_.tags = editedTrackFromUser.tags
              track_.year = editedTrackFromUser.year
            }
            else
              draft.fromMusicDb!.data = draft.fromMusicDb!.data.filter(t => t.youtubeVideoId !== editedTrackFromUser.trackYtId)
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