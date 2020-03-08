import { useAxios } from "./axios"
import { AdminSectionParams, AdminCommand } from "../shared/admin"

export const useAdminApi = () => {
  const { get, post, put } = useAxios()

  const getInitialParams = async () => get<AdminSectionParams>('/admin').then(r => r.data)

  const addCommand = async (cmd: Omit<AdminCommand, 'id'>) => (await post<AdminCommand>('/admin', cmd)).data

  const updateCommand = (cmd: AdminCommand) => put('/admin', cmd)

  return { getInitialParams, addCommand, updateCommand }
}