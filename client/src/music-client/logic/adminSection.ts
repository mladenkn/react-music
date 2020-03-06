import { useAdminApi } from "../api/adminApi"
import { AdminCommand } from "../shared/admin"
import { useEffect } from "react"
import { Loadable, Loaded } from "../../utils/types"
import { useImmer } from "use-immer"
import { useDebouncedCallback } from "use-debounce/lib"

interface State {
  activeCommandName: string
  activeCommandYaml: string
  activeCommandResponseYaml: Loadable<string>
  commands: AdminCommand[]
}

interface AdminSectionLogic {
  activeCommand: AdminCommand
  activeCommandResponseYaml: Loadable<string>
  commands: AdminCommand[]
  setActiveCommand(cmdName: string): void
  updateCommandYaml(yaml: string): void
  updateCommandName(newName: string): void
}

const initalCmdResponse = `doe: 'a deer, a female deer'
ray: 'a drop of golden sun'
pi: 3.14159
xmas: true
french-hens: 3
calling-birds: 
  - huey
  - dewey
  - louie
  - fred
xmas-fifth-day: 
  calling-birds: four
  french-hens: 3
  golden-rings: 5
  partridges: 
    count: 1
    location: 'a pear tree'
  turtle-doves: two`

export const useAdminSectionLogic = (): Loadable<AdminSectionLogic> => {

  const { getInitialParams, saveCommand } = useAdminApi()
  

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
            activeCommandYaml: response.commands.find(c => c.name === response.currentCommandName)!.yaml,
            activeCommandResponseYaml: { type: 'LOADED', data: initalCmdResponse },
            commands: response.commands,
          }
        }))
      })
  }, [])

  const updateCommand = (cmd: AdminCommand) => {
    saveCommand(cmd)
      .then(() => {
        updateState(draft => {
          const draftC = draft as Loaded<State>
          const cmd_ = draftC.data.commands.find(
            q => q.name === draftC.data.activeCommandName
          )!
          Object.assign(cmd_, cmd)
        })
      })
  }

  const [updateCommandYamlOnApi] = useDebouncedCallback((cmd: AdminCommand) => updateCommand(cmd), 1000)

  if(state.type === 'LOADING')
    return { type: 'LOADING' }

  else if (state.type === 'LOADED') {
    const setActiveCommand = (name: string) => {
      updateState(draft => {
        (draft as Loaded<State>).data.activeCommandName = name
      })
    }

    const activeCommand = state.data.commands.find(q => q.name === state.data.activeCommandName)!

    const updateCommandName = (name: string) => {
      updateCommand({ ...activeCommand, name })
    }

    const updateCommandYaml = (yaml: string) => {
      updateState(draft => {
        (draft as Loaded<State>).data.activeCommandYaml = yaml
      })
      updateCommandYamlOnApi({...activeCommand, yaml})
    }

    return {
      type: 'LOADED',
      data: {
        activeCommand: {
          name: state.data.activeCommandName,
          yaml: state.data.activeCommandYaml
        },
        activeCommandResponseYaml: state.data.activeCommandResponseYaml,
        commands: state.data.commands,
        setActiveCommand,
        updateCommandName,
        updateCommandYaml
      }
    }
  }

  else 
    return { type: 'ERROR' }
}
