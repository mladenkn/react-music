import { useAxios } from "./axios"
import { AdminSectionParams, AdminCommand } from "../shared/admin"

export const useAdminApi = () => {
  const { get, post } = useAxios()

  const getInitialParams = async () => get<AdminSectionParams>('/admin').then(r => r.data)

  const updateCommand = async (cmd: AdminCommand) => post('/admin', cmd)

  return { getInitialParams, updateCommand }
}