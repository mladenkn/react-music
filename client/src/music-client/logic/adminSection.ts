import { useAdminApi } from "../api/adminApi"
import { AdminCommand } from "../shared/admin"
import { Loadable, Loaded } from "../../utils/types"
import { useImmer } from "use-immer"
import { useDebouncedCallback } from "use-debounce/lib"
import { useEffect } from "../../utils/useEffect"

interface State {
  activeCommandId: number
  activeCommandYaml: string
  activeCommandResponseYaml: Loadable<string>
  commands: AdminCommand[]
}

interface AdminSectionLogic {
  activeCommand: AdminCommand
  activeCommandResponseYaml: Loadable<string>
  commands: AdminCommand[]
  setActiveCommand(cmdId: number): void
  updateCommandYaml(yaml: string): void
  updateCommandName(newName: string): void
  addNewCommand(name: string): void
  executeCommand(): void
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

  const api = useAdminApi()  

  const [state, updateState] = useImmer<Loadable<State>>({
    type: "LOADING"
  })
  
  console.log(state)

  useEffect(() => {
    api.getInitialParams()
      .then(response => {
        const activeCommandId = response.currentCommandId || response.commands[0].id
        updateState(() => ({
          type: 'LOADED',
          data: {
            activeCommandId,
            activeCommandYaml: response.commands.find(c => c.id === activeCommandId)!.yaml,
            activeCommandResponseYaml: { type: 'LOADED', data: initalCmdResponse },
            commands: response.commands,
          }
        }))
      })
  }, [], { runOnFirstRender: true })

  useEffect(() => {
    api.persistAdminSectionState({ currentCommandId: (state as Loaded<State>).data.activeCommandId })
  }, [state.type == 'LOADED' && state.data.activeCommandId])

  const updateCommand = (cmd: AdminCommand) => {
    api.updateCommand(cmd)
      .then(() => {
        updateState(draft_ => {
          const draft = (draft_ as Loaded<State>).data
          const cmd_ = draft.commands.find(q => q.id === draft.activeCommandId)!
          cmd_.name = cmd.name
          cmd_.yaml = cmd.yaml
        })
      })
  }

  const [updateCommandDebounced] = useDebouncedCallback(updateCommand, 1000)

  if(state.type === 'LOADING')
    return { type: 'LOADING' }

  else if (state.type === 'LOADED') {
    const setActiveCommand = (id: number) => {
      updateState(draft_ => {
        const draft = (draft_ as Loaded<State>).data
        draft.activeCommandId = id
        draft.activeCommandYaml = draft.commands.find(c => c.id === id)!.yaml
      })
    }

    const activeCommand = state.data.commands.find(q => q.id === state.data.activeCommandId)!

    const updateCommandName = (name: string) => {
      updateCommand({ id: activeCommand.id, name, yaml: state.data.activeCommandYaml })
    }

    const updateCommandYaml = (yaml: string) => {
      updateState(draft => {
        (draft as Loaded<State>).data.activeCommandYaml = yaml
      })
      updateCommandDebounced({ ...activeCommand, yaml})
    }

    const addNewCommand = (name: string) => {
      const cmd = {name, yaml: ''}
      api.addCommand(cmd)
        .then((cmdFromApi) => {
          updateState(draft_ => {
            const draft = (draft_ as Loaded<State>).data
            draft.activeCommandId = cmdFromApi.id
            draft.activeCommandResponseYaml = { type: 'LOADED', data: '' }
            draft.activeCommandYaml = ''
            draft.commands.unshift(cmdFromApi)
          }) 
        })
    }

    const executeCommand = () => {
      updateState(draft_ => {
        const draft = (draft_ as Loaded<State>).data
        draft.activeCommandResponseYaml = { type: 'LOADING' }        
      })
      api.executeCommand(state.data.activeCommandYaml)
        .then(responseYaml => {
          updateState(draft_ => {
            const draft = (draft_ as Loaded<State>).data
            draft.activeCommandResponseYaml = { type: 'LOADED', data: responseYaml }  
          })
        })
    }

    return {
      type: 'LOADED',
      data: {
        activeCommand: {
          ...activeCommand,
          yaml: state.data.activeCommandYaml
        },
        addNewCommand,
        activeCommandResponseYaml: state.data.activeCommandResponseYaml,
        commands: state.data.commands,
        setActiveCommand,
        updateCommandName,
        updateCommandYaml,
        executeCommand
      }
    }
  }

  else 
    return { type: 'ERROR' }
}
