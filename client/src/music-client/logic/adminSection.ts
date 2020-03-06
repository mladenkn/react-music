import { useImmer } from "use-immer"
import { useAdminApi } from "../api/adminApi"
import { AdminYamlCommand } from "../shared/admin"
import { useEffect, useState } from "react"
import { Loaded, Loadable } from "../../utils/types"

interface UnloadedState {
  type: 'UnloadedState',
}

interface LoadedState {
  type: 'LoadedState',  
  activeCommandName: string
  activeCommandResponseYaml: Loadable<string>
  commands: AdminYamlCommand[]
}

interface LoadedAdminSectionLogic {
  type: 'LoadedAdminSectionLogic'
  activeCommand: AdminYamlCommand
  activeCommandResponseYaml: Loadable<string>
  commands: AdminYamlCommand[]
  setActiveCommand(cmdName: string): void
}

export const useAdminSectionLogic = (): UnloadedState | LoadedAdminSectionLogic => {

  const { getInitialParams } = useAdminApi()

  const [state, updateState] = useState<UnloadedState | LoadedState>({
    type: "UnloadedState"
  })

  useEffect(() => {
    getInitialParams()
      .then(response => {
        updateState({
          type: 'LoadedState',
          activeCommandName: response.currentCommandName,
          activeCommandResponseYaml: response.currentCommandResponse,
          commands: response.commands
        } as any)
      })
  })

  if(state.type === 'UnloadedState')
    return { type: 'UnloadedState' }
  else {
    const setActiveCommand = (name: string) => {
      updateState(curState => ({ ...curState, activeCommandName: name }))
    }
    const activeCommand = state.commands.find(q => q.name === state.activeCommandName)!    
    return {
      type: 'LoadedAdminSectionLogic',
      activeCommand,
      activeCommandResponseYaml: state.activeCommandResponseYaml,
      commands: state.commands,
      setActiveCommand,
    }
  }
}