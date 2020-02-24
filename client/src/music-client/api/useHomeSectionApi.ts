import { useAxios } from "./axios"
import { HomeSectionOptions } from "../shared/homeSectionOptions"

export const useHomeSectionApi = () => {

  const { post } = useAxios()  

  const saveOptions = (opt: HomeSectionOptions) => {
    return post('/homeSection', opt)
  }

  return { saveOptions }
}