import { useAxios } from "./axios"
import { HomeSectionPersistableState, HomeSectionPropsFromApi } from "../shared/homeSectionOptions"

export const useHomeSectionApi = () => {

  const { get, post } = useAxios()  

  const saveOptions = (opt: HomeSectionPersistableState) => {
    return post('/homeSection', opt)
  }

  const getProps = () => {
    return get<HomeSectionPropsFromApi>('/homeSection/props')
  }

  return { saveOptions, getProps }
}