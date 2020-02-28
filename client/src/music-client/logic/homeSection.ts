import { useTracklistLogic } from "./tracklist"
import { useImmer } from "use-immer";
import { HomeSectionOptions, HomeSectionPropsFromApi } from "../shared/homeSectionOptions";
import { useDebouncedCallback } from "use-debounce/lib";
import { useHomeSectionApi } from "../api/homeSection";
import { useEffect } from "react";

export const useHomeLogic = (props: HomeSectionPropsFromApi) => {
  const [state, updateState] = useImmer({
    currentTrackId: undefined as undefined | number,
    options: props.options
  })
  
  const tracklist = useTracklistLogic({
    options: state.options.tracklist,
    tracksFromMusicDb: props.tracksFromMusicDb,
    tracksFromYouTube: props.tracksFromYouTube,
    selectedTrackId: props.selectedTrackId
  });

  const api = useHomeSectionApi();

  const [saveState] = useDebouncedCallback(() => {
    api.saveState({ 
      options: state.options, 
      currentTrackId: state.currentTrackId, 
      selectedTrackId: tracklist.selectedTrackId 
    })
  }, 2000)

  useEffect(saveState, [state.options, state.currentTrackId, tracklist.selectedTrackId])

  function setCurrentTrack(trackId: number){
    updateState(draft => {
      draft.currentTrackId = trackId
    })
  }

  function onCurrentTrackFinish(){
    if(!tracklist.options.autoPlay)
      return
    const curTrackIndex = tracklist.tracks!.findIndex(t => t.id === state.currentTrackId!)
    const maybeNextTrack = tracklist.tracks![curTrackIndex + 1]
    const nextTrackId = maybeNextTrack ? maybeNextTrack.id : tracklist.tracks![0].id
    updateState(draft => {
      draft.currentTrackId = nextTrackId
    })
  }

  function setOptions(opt: HomeSectionOptions){
    updateState(draft => {
      draft.options = opt
    })
  }

  const currentTrack = tracklist.tracks && state.currentTrackId && tracklist.tracks!.find(t => t.id === state.currentTrackId)
  const currentTrackYoutubeId = currentTrack && currentTrack!.youTubeVideoId

  return { ...tracklist, ...state, currentTrackYoutubeId, setCurrentTrack, onCurrentTrackFinish, setOptions }
}
