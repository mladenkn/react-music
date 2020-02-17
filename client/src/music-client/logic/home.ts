import { useTracklistLogic } from "./tracklist"
import { mapToTrackViewModel, TrackViewModel } from "../shared/track";

export const useHomeLogic = () => {
  const tracklist = useTracklistLogic(); 

  let tracks: TrackViewModel[] | undefined
  if(tracklist.fromMusicDb || tracklist.fromYouTube){
    const beforeMap = tracklist.fromMusicDb ? tracklist.fromMusicDb.data : tracklist.fromYouTube!
    tracks = beforeMap.map(track => mapToTrackViewModel(track, tracklist.selectedTrackId))
  }
  else
    tracks = undefined  

  const tracksTotalCount = tracklist.fromMusicDb && tracklist.fromMusicDb.totalCount

  return { ...tracklist, tracks, tracksTotalCount }
}