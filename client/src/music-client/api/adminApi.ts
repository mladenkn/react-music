import { useAxios } from "./axios"
import { AdminSectionParams } from "../shared/admin"

export const useAdminApi = () => {
  const { get } = useAxios()

  const getInitialParams = async () => {
    return get<AdminSectionParams>('/admin').then(r => r.data)
  }

  return { getInitialParams }
}