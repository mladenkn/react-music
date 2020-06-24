import { useAxios } from "./axios"
import { AdminSectionParams,  AdminSectionPersistableState, CsCommand } from "../shared/admin"

export const useAdminApi = () => {
  const { get, post, put } = useAxios()

  const getInitialParams = async () => get<AdminSectionParams>('/admin/section/props').then(r => r.data)
  const persistCsCommandsSectionState = (s: AdminSectionPersistableState) => post('/admin/section/props', s)

  const addCommand = async (cmd: Omit<CsCommand, 'id'>) => (await post<CsCommand>('/admin/commands', cmd)).data
  const updateCommand = (cmd: CsCommand) => put('/admin/commands', cmd)
  const executeCommand = (command: unknown) => post<string>('admin/commands/execute', command)
  const executeCsCommand = (command: string) => post<string>('admin/cs-commands/execute', {command})

  const setVaraiable = (key: string, value: unknown) => post('admin/variables', {key, value})

  return { getInitialParams, addCommand, updateCommand, persistCsCommandsSectionState, executeCommand, setVaraiable, executeCsCommand }
}