import { Track, SaveTrackModel, mapToTrackViewModel, TrackViewModel } from "../shared/track";
import { ArrayWithTotalCount } from "../../utils/types";
import { useImmer } from "use-immer";
import { TracklistOptions, TrackQueryFormType } from "../shared/homeSectionOptions";
import { useDebouncedCallback } from 'use-debounce';
import { useTracksApi } from "../api/tracks";
import { useEffect } from "../../utils/useEffect";
import { uniqBy } from 'lodash';

export interface Tracklist {
  options: TracklistOptions
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: number
  tracks: TrackViewModel[] | undefined
  tracksTotalCount?: number
  fetchTracksNextPage(): void
  saveTrack(t: SaveTrackModel): Promise<void>
  onTrackClick(trackId: number): void
  fetchTracks(): void
}

interface State {
  fromMusicDb?: ArrayWithTotalCount<Track>
  fromYouTube?: Track[]
  selectedTrackId?: number
}

interface TracklistProps {
  options: TracklistOptions
  tracksFromMusicDb?: ArrayWithTotalCount<Track>
  tracksFromYouTube?: Track[]
  selectedTrackId?: string
}

const pageSize = 30;

export const useTracklistLogic = (props: TracklistProps): Tracklist => {

  const [state, updateState] = useImmer<State>({
    fromMusicDb: props.tracksFromMusicDb,
    fromYouTube: props.tracksFromYouTube
  })

  const api = useTracksApi()

  useEffect(() => {
    if(props.options.autoRefresh)
      refetchOnChange()
  }, [props.options.query])

  const [refetchOnChange] = useDebouncedCallback(() => fetch(), 700)

  const { query: queryForm } = props.options

  async function fetch(){
    updateState(draft => {
      draft.fromYouTube = undefined
      draft.fromMusicDb = undefined
    })
    if(queryForm.dataSource === TrackQueryFormType.MusicDb){
      const { data } = await api.fetchFromMusicDb({ ...queryForm.musicDbQuery!, skip: 0, take: pageSize })
      updateState(draft => {
        draft.fromYouTube = undefined
        draft.fromMusicDb = data
      })
    } 
    else if(queryForm.dataSource === TrackQueryFormType.YouTube){
      const { data } = await api.fetchFromYouTube(queryForm.youTubeQuery!)
      updateState(draft => {
        draft.fromMusicDb = undefined
        draft.fromYouTube = data
      })
    }
  }

  async function fetchTracksNextPage(){
    const skip = state.fromMusicDb!.data.length
    const response = await api.fetchFromMusicDb({ ...queryForm.musicDbQuery!, skip, take: pageSize })
    const tracks = uniqBy([ ...state.fromMusicDb!.data, ...response.data.data ], t => t.id)
    updateState(draft => {
      draft.fromMusicDb = { data: tracks, totalCount: response.data.totalCount }
    })
  }

  function doesPassCurrentFilter(track: { tags: string[], year: number }){
    if(queryForm.dataSource === TrackQueryFormType.YouTube)
      return true
    else {
      const filter = queryForm.musicDbQuery!
      if(filter.mustHaveAnyTag && filter.mustHaveAnyTag.length > 0){
        const hasAny = track.tags.some(t => filter.mustHaveAnyTag.includes(t))
        if(!hasAny)
          return false
      }
      if(filter.mustHaveEveryTag && filter.mustHaveEveryTag.length > 0){        
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
      api.save(editedTrackFromUser)
        .then(() => {
          resolve()
          updateState(draft => {
            if(doesPassCurrentFilter(editedTrackFromUser)){
              const track_ = (draft.fromYouTube || draft.fromMusicDb!.data).find(t => t.id === editedTrackFromUser.trackId)!
              track_.tags = editedTrackFromUser.tags
              track_.year = editedTrackFromUser.year
            }
            else
              draft.fromMusicDb!.data = draft.fromMusicDb!.data.filter(t => t.id !== editedTrackFromUser.trackId)
          })
        })
        .catch(() => {
          reject()
        })
    })
  }
  
  function onTrackClick(trackId: number) {
    updateState(draft => {
      draft.selectedTrackId = trackId
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
    options: props.options,
    tracks,
    tracksTotalCount,    
    fetchTracks: fetch, 
    fetchTracksNextPage, 
    saveTrack, 
    onTrackClick,
  }
}
