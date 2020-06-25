export interface AdminSectionParams {
  commands: CsCommand[]
  currentCommandId?: number
  currentCommandResponse: string
}

export interface AdminSectionPersistableState {
  currentCommandId: number
}

export interface CsCommand {
  id: number
  name: string
  code: string  
}

export interface CsCommandsState {
  commands: CsCommand[]
  currentCommandId?: number
  currentCommandResponse: string
}