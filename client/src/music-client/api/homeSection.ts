import { useAxios } from "./axios"
import { HomeSectionPersistableState, HomeSectionPropsFromApi } from "../shared/homeSectionOptions"

export const useHomeSectionApi = () => {
  const { post } = useAxios()

  const { get, post } = useAxios()  

  const saveState = (opt: HomeSectionPersistableState) => {
    return post('/homeSection', opt)
  }

  const getProps = () => {
    return get<HomeSectionPropsFromApi>('/homeSection/props')
  }

  return { saveState, getProps }
}
