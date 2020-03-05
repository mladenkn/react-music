import { useAxios } from "./axios"
import { AdminYamlCommand } from "../shared/admin"

export const useAdminApi = () => {
  const { get } = useAxios()

  const getCommands = async () => {
    return get<AdminYamlCommand[]>('/admin').then(r => r.data)
  }

  return { getCommands }
}