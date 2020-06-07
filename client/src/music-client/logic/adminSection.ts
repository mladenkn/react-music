import { useAdminApi } from "../api/adminApi"
import { AdminCommand } from "../shared/admin"
import { Loadable, Loaded } from "../../utils/types"
import { useImmer } from "use-immer"
import { useDebouncedCallback } from "use-debounce/lib"
import { useEffect } from "../../utils/useEffect"
import yaml from 'js-yaml'

interface State {
  activeCommandId: number
  activeCommandYaml: string
  activeCommandResponseYaml: Loadable<string>
  jsMapperYaml: string
  commands: AdminCommand[]
}

interface AdminSectionLogic {
  activeCommand: AdminCommand
  jsMapperYaml: string
  activeCommandResponseYaml: Loadable<string>
  commands: AdminCommand[]
  setActiveCommand(cmdId: number): void
  updateCommandYaml(yaml: string): void
  updateJsMapperYaml(yaml: string): void
  updateCommandName(newName: string): void
  addNewCommand(name: string): void
  executeCommand(): void
}

const initalCmdResponse = ''

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
            jsMapperYaml: '',
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

    const updateJsMapperYaml = (yaml: string) => {
      updateState(draft => {
        (draft as Loaded<State>).data.jsMapperYaml = yaml
      })
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
      
      let commandObj
      try {
        commandObj = yaml.safeLoad(state.data.activeCommandYaml)
      }
      catch(error){
        updateState(draft_ => {
          const draft = (draft_ as Loaded<State>).data
          draft.activeCommandResponseYaml = {
            type: 'LOADED',
            data: `User YAML error: ${error.message}`
          }
        })
        return
      }

      api.executeCommand(commandObj)
        .then(response => {
          const responseYaml = yaml.safeDump(response.data)
          updateState(draft_ => {
            const draft = (draft_ as Loaded<State>).data
            if(state.data.jsMapperYaml !== ''){
              draft.activeCommandResponseYaml = {
                type: 'LOADED',
                data: mapResponse(responseYaml, state.data.jsMapperYaml)
              }
            } 
            else           
              draft.activeCommandResponseYaml = { type: 'LOADED', data: responseYaml }  
          })
        })
        .catch(error => {        
          updateState(draft_ => {
            const draft = (draft_ as Loaded<State>).data
            console.log(error)
            if(error.response.status >= 400 && error.response.status < 500)
              draft.activeCommandResponseYaml = {
                type: 'LOADED',
                data: 'Bad command'
              }
            else
              draft.activeCommandResponseYaml = {
                type: 'LOADED',
                data: `Error: ${error.response.data}`
              }
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
        jsMapperYaml: state.data?.jsMapperYaml,
        setActiveCommand,
        updateCommandName,
        updateCommandYaml,
        executeCommand,
        updateJsMapperYaml
      }
    }
  }

  else 
    return { type: 'ERROR' }
}

const mapResponse = (responseYaml: string, jsMapper: string) => {
  const responseObject = yaml.safeLoad(responseYaml)
  const code = `
      (function(){
        var response = ${JSON.stringify(responseObject)}
        ${jsMapper}
      })()
    `
    try {
      return yaml.safeDump(eval(code))
    }
    catch (error) {
      return `'Error in js mapper: ${error.message}'`
    }
}