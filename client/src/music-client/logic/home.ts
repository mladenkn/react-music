import { useTracklistLogic } from "./tracklist"
import { useImmer } from "use-immer";
import { createInitialHomeSectionOptions } from "../shared/homeSectionOptions";

export const useHomeLogic = () => {
  const [state, updateState] = useImmer({
    currentTrackYoutubeId: undefined as string | undefined,
    options: createInitialHomeSectionOptions()
  })

  const tracklist = useTracklistLogic({
    options: state.options.tracklist,
    onOptionsChange: tracklistOptions => {
      updateState(draft => {
        draft.options.tracklist = tracklistOptions
      })
    }
  });

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

  return { ...tracklist, ...state, setCurrentTrack, onCurrentTrackFinish }
}