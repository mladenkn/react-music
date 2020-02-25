import { useAxios } from "./axios"
import { HomeSectionPersistableState, HomeSectionPropsFromApi, TrackQueryFormDataSource } from "../shared/homeSectionOptions"
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

  const getProps = async () => {
    const response = await get<HomeSectionPropsFromApi>('/homeSection/props')
    return produce(response, draft => {
      response.data.options.tracklist.queryForm.dataSource = 
        response.data.options.tracklist.queryForm.musicDbQuery == null ?
          TrackQueryFormDataSource.YouTube :
          TrackQueryFormDataSource.MusicDb
    })
  }

  return { saveOptions, getProps }
}