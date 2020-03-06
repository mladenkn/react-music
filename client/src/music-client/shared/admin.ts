export interface AdminCommand {
  name: string
  yaml: string
}

export interface AdminSectionParams {
  commands: AdminCommand[]
  currentCommandName: string
  currentCommandResponse: string
}

