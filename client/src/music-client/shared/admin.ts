export interface AdminYamlCommand {
  name: string
  yaml: string
}

export interface AdminSectionParams {
  commands: AdminYamlCommand[]
  currentCommandName: string
  currentCommandResponse: string
}

