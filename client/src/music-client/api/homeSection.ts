import { useAxios } from "./axios"
import { HomeSectionPersistableState, HomeSectionPropsFromApi } from "../shared/homeSectionOptions"
import { produce } from 'immer'

export const useHomeSectionApi = () => {

  const { get, post } = useAxios()  

  const saveOptions = (opt: HomeSectionPersistableState) => {
    const mapped = produce(opt, draft => {
      if(draft.options.tracklist.queryForm.dataSource === "MusicDb")
        draft.options.tracklist.queryForm.youTubeQuery = undefined
      else
        draft.options.tracklist.queryForm.musicDbQuery = undefined
    });
    return post('/homeSection', mapped)
  }

  const getProps = () => {
    return get<HomeSectionPropsFromApi>('/homeSection/props')
  }

  return { saveOptions, getProps }
}