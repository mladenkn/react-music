import { useTracklistLogic } from "./tracklist"

export const useHomeLogic = () => {
  const tracklist = useTracklistLogic();
  return { ...tracklist }
}