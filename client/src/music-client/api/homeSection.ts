import { useAxios } from "./axios"
import { HomeSectionOptions } from "../shared/homeSectionOptions"
import { produce } from 'immer'

export const useHomeSectionApi = () => {

  const { post } = useAxios()  

  const saveOptions = (opt: HomeSectionOptions) => {
    const mapped = produce(opt, draft => {
      if(draft.tracklist.queryForm.dataSource === "MusicDb")
        draft.tracklist.queryForm.youTubeQuery = undefined
      else
        draft.tracklist.queryForm.musicDbQuery = undefined
    });
    return post('/homeSection', mapped)
  }

  return { saveOptions }
}