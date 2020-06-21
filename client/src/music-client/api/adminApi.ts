import { useAxios } from "./axios"
import { AdminSectionParams, AdminCommand, AdminSectionPersistableState } from "../shared/admin"

export const useAdminApi = () => {
  const { get, post, put } = useAxios()

  const getInitialParams = async () => get<AdminSectionParams>('/admin/section/props').then(r => r.data)
  const persistAdminSectionState = (s: AdminSectionPersistableState) => post('/admin/section/props', s)

  const addCommand = async (cmd: Omit<AdminCommand, 'id'>) => (await post<AdminCommand>('/admin/commands', cmd)).data
  const updateCommand = (cmd: AdminCommand) => put('/admin/commands', cmd)
  const executeCommand = (command: unknown) => post<string>('admin/commands/execute', command)

  const setVaraiable = (key: string, value: unknown) => post('admin/variables', {key, value})

  return { getInitialParams, addCommand, updateCommand, persistAdminSectionState, executeCommand, setVaraiable }
}