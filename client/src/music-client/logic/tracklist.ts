import { Track } from "../shared/track";
import { ArrayWithTotalCount } from "../../utils/types";
import { useImmer } from "use-immer";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { TrackQueryForm, createInitialTrackQueryForm, TrackQueryFormDataSource } from "../shared/trackQueryForm";

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

const trackQueryFormTestInitialValues: TrackQueryForm = {
	dataSource: TrackQueryFormDataSource.MusicDb,
	musicDbParams: {
		mustHaveAnyTag: ['trance', 'techno'],
		mustHaveEveryTag: ['house', 'acid'],
		titleContains: 'mate i jure',
		youtubeChannelId: undefined,
		yearRange: {
			lowerBound: 1990,
			upperBound: 1998
		}
	},
	searchQuery: 'mate i frane',
	autoRefresh: true,
}

export const useTracklistLogic = (): Tracklist => {

  const [state, updateState] = useImmer<State>({
    queryForm: trackQueryFormTestInitialValues,
  })

  useEffect(() => {
    fetch(state.queryForm)
  }, [state.queryForm])

  async function fetch(form: TrackQueryForm){
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