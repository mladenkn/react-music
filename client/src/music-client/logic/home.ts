import { useTracklistLogic } from "./tracklist"
import { useImmer } from "use-immer";

export const useHomeLogic = () => {
  const tracklist = useTracklistLogic();

  const [state, updateState] = useImmer({
    currentTrackYoutubeId: undefined as string | undefined
  })

  function setCurrentTrack(trackYoutubeId: string){
    updateState(draft => {
      draft.currentTrackYoutubeId = trackYoutubeId
    })
  }

  function onCurrentTrackFinish(){
    const curTrackIndex = tracklist.tracks!.findIndex(t => t.youtubeVideoId === state.currentTrackYoutubeId!)
    const maybeNextTrack = tracklist.tracks![curTrackIndex + 1]
    const nextTrackId = maybeNextTrack ? maybeNextTrack.youtubeVideoId : tracklist.tracks![0].youtubeVideoId
    updateState(draft => {
      draft.currentTrackYoutubeId = nextTrackId
    })
  }

  return { ...tracklist, ...state, setCurrentTrack, onCurrentTrackFinish }
}