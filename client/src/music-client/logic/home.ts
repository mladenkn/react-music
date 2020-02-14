import { useTracklistLogic } from "./tracklist-new/tracklist"

export const useHomeLogic = () => {
  const tracklist = useTracklistLogic();
  return { ...tracklist }
}