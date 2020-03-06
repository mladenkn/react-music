import { useAdminApi } from "../api/adminApi"
import { AdminYamlCommand } from "../shared/admin"
import { useEffect, useState } from "react"
import { Loadable, Loaded } from "../../utils/types"
import { useImmer } from "use-immer"

interface State {
  activeCommandName: string
  activeCommandResponseYaml: Loadable<string>
  commands: AdminYamlCommand[]
}

interface AdminSectionLogic {
  activeCommand: AdminYamlCommand
  activeCommandResponseYaml: Loadable<string>
  commands: AdminYamlCommand[]
  setActiveCommand(cmdName: string): void
}

export const useAdminSectionLogic = (): Loadable<AdminSectionLogic> => {

  const { getInitialParams } = useAdminApi()
  

  const [state, updateState] = useImmer<Loadable<State>>({
    type: "LOADING"
  })
  
  console.log(state)

  useEffect(() => {
    getInitialParams()
      .then(response => {
        updateState(() => ({
          type: 'LOADED',
          data: {
            activeCommandName: response.currentCommandName,
            activeCommandResponseYaml: { type: 'LOADED', data: response.currentCommandResponse },
            commands: response.commands
          }
        }))
      })
  }, [])

  if(state.type === 'LOADING')
    return { type: 'LOADING' }

  else if (state.type === 'LOADED') {
    const setActiveCommand = (name: string) => {
      updateState(draft => {
        (draft as Loaded<State>).data.activeCommandName = name
      })
    }
    const activeCommand = state.data.commands.find(q => q.name === state.data.activeCommandName)!
    return {
      type: 'LOADED',
      data: {
        activeCommand,
        activeCommandResponseYaml: state.data.activeCommandResponseYaml,
        commands: state.data.commands,
        setActiveCommand,
      }
    }
  }

  else 
    return { type: 'ERROR' }
}