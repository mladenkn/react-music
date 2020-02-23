import { useTracklistLogic } from "./tracklist"
import { useImmer } from "use-immer";
import { createInitialHomeSectionOptions, HomeSectionOptions } from "../shared/homeSectionOptions";
import { useHomeSectionApi } from "../api/homeSection";
import { useEffect } from "react";

export const useHomeLogic = () => {
  const [state, updateState] = useImmer({
    currentTrackYoutubeId: undefined as string | undefined,
    options: createInitialHomeSectionOptions()
  })

  const tracklist = useTracklistLogic({
    options: state.options.tracklist
  });

  const api = useHomeSectionApi()

  useEffect(() => {
    api.saveOptions(state.options)
  }, [state.options])

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