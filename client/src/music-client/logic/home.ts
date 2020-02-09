import { useTracklistLogic } from "./tracklist.decorated"

export const useHomeLogic = () => {
  const tracklist = useTracklistLogic();
  return { tracklist }
}