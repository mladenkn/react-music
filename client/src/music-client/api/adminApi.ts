import { useAxios } from "./axios"
import { AdminSectionParams, AdminCommand, AdminSectionPersistableState } from "../shared/admin"

export const useAdminApi = () => {
  const { get, post, put } = useAxios()

  const getInitialParams = async () => get<AdminSectionParams>('/admin/admin-section').then(r => r.data)

  const addCommand = async (cmd: Omit<AdminCommand, 'id'>) => (await post<AdminCommand>('/admin', cmd)).data

  const updateCommand = (cmd: AdminCommand) => put('/admin', cmd)

  const persistAdminSectionState = (s: AdminSectionPersistableState) => post('/admin/admin-section', s)

  const executeCommand = async (commandYaml: string) => 
    (await post<string>('admin/exe-command', {commandYaml})).data

  return { getInitialParams, addCommand, updateCommand, persistAdminSectionState, executeCommand }
}