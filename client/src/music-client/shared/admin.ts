export interface AdminCommand {
  id: number
  name: string
  yaml: string
}

export interface AdminSectionParams {
  commands: AdminCommand[]
  currentCommandId: number
  currentCommandResponse: string
}
