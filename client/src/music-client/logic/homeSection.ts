import { useTracklistLogic } from "./tracklist"
import { useImmer } from "use-immer";
import { HomeSectionOptions, HomeSectionPropsFromApi } from "../shared/homeSectionOptions";
import { useDebouncedCallback } from "use-debounce/lib";
import { useHomeSectionApi } from "../api/homeSection";
import { useEffect } from "react";

export const useHomeLogic = (props: HomeSectionPropsFromApi) => {
  const [state, updateState] = useImmer({
    currentTrackYoutubeId: undefined as undefined | string,
    options: props.options
  })
  
  const tracklist = useTracklistLogic({
    options: state.options.tracklist,
    tracksFromMusicDb: props.tracksFromMusicDb,
    tracksFromYouTube: props.tracksFromYouTube,
    selectedTrackId: props.selectedTrackYoutubeId
  });

  const api = useHomeSectionApi();

  const [saveState] = useDebouncedCallback(() => {
    api.saveState({ 
      options: state.options, 
      currentTrackYoutubeId: state.currentTrackYoutubeId, 
      selectedTrackYoutubeId: tracklist.selectedTrackId 
    })
  }, 2000)

  useEffect(saveState, [state.options, state.currentTrackYoutubeId, tracklist.selectedTrackId])

  function setCurrentTrack(trackYoutubeId: string){
    updateState(draft => {
      draft.currentTrackYoutubeId = trackYoutubeId
    })
  }

  function onCurrentTrackFinish(){
    if(!tracklist.options.autoPlay)
      return
    const curTrackIndex = tracklist.tracks!.findIndex(t => t.youtubeVideoId === state.currentTrackYoutubeId!)
    const maybeNextTrack = tracklist.tracks![curTrackIndex + 1]
    const nextTrackId = maybeNextTrack ? maybeNextTrack.youtubeVideoId : tracklist.tracks![0].youtubeVideoId
    updateState(draft => {
      draft.currentTrackYoutubeId = nextTrackId
    })
  }

  function setOptions(opt: HomeSectionOptions){
    updateState(draft => {
      draft.options = opt
    })
  }

  return { ...tracklist, ...state, setCurrentTrack, onCurrentTrackFinish, setOptions }
}
