import { Loadable } from "../../utils/types";
import { CsCommand } from "../shared/admin";
import { useImmer } from "use-immer";
import { useAdminApi } from "../api/adminApi";
import { useDebouncedCallback } from "use-debounce/lib";
import { useEffect } from "../../utils/useEffect";
import { Draft } from "immer";

interface State {
  activeCommandId?: number
  activeCommandCode?: string
  activeCommandResponse: Loadable<unknown>
  commands: CsCommand[]  
}

export interface CsCommandsLogic {
  activeCommand?: CsCommand
  activeCommandResponse: Loadable<unknown>
  commands: CsCommand[]
  setActiveCommand(cmdId: number): void
  updateCommandCode(code: string): void
  updateCommandName(newName: string): void
  addNewCommand(name: string): void
  executeCommand(): void
}

export function useCsCommands(): Loadable<CsCommandsLogic> {

  const api = useAdminApi()

  const [state, updateState] = useImmer<Loadable<State>>({
    type: 'LOADING',
  })

  console.log(state)

  useEffect(() => {
    api.getInitialParams()
      .then(response => {
        const activeCommandId = response.currentCommandId || response.commands[0]?.id
        updateState(() => ({
          type: 'LOADED',
          value: {
            activeCommandId,
            activeCommandCode: response.commands.find(c => c.id === activeCommandId)?.code,
            activeCommandResponse: { type: 'LOADED', value: '' },
            commands: response.commands,
            savedToVariableMessageShown: false,
          }
        }))
      })
  }, [], { runOnFirstRender: true })

  const activeCommand = 
    state.type === 'LOADED' && state.value.activeCommandId ?
    {
      id: state.value.activeCommandId,
      name: state.value.commands.find(c => c.id === state.value.activeCommandId)!.name,
      code: state.value.activeCommandCode!
    } :
    undefined

  function updateLoadedState(mutate: (draft: Draft<State>) => void){
    updateState(draft => {
      if(draft.type !== 'LOADED')
        throw new Error()
      else
        mutate(draft.value)      
    })
  }

  function updateCommand(cmd: CsCommand) {
    api.updateCommand(cmd)
      .then(() => {
        updateLoadedState(draft => {
          const cmd_ = draft.commands.find(q => q.id === draft.activeCommandId)!
          cmd_.name = cmd.name
          cmd_.code = cmd.code
        })
      })
  }

  function setActiveCommand(cmdId: number) {      
    updateState(draft => {
      if(draft.type !== 'LOADED')
        throw new Error()
      draft.value.activeCommandId = cmdId
      draft.value.activeCommandCode = draft.value.commands.find(c => c.id === cmdId)!.code
    })
  }
  
  function updateCommandCode(code: string) {
    if(state.type !== 'LOADED' || !activeCommand)
      throw new Error()
    updateState(draft => {
      if(draft.type !== 'LOADED' || !activeCommand)
        throw new Error()
      draft.value.activeCommandCode = code
    })
    updateCommandDebounced({ ...activeCommand, code })
  }
  
  function updateCommandName(newName: string) {
    if(state.type !== 'LOADED' || !activeCommand)
      throw new Error()
    updateCommand({ ...activeCommand, name: newName })
  }
  
  function addNewCommand(name: string) {
    const cmd = { name, code: '' }
    api.addCommand(cmd)
      .then((cmdFromApi) => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          draft.value.activeCommandId = cmdFromApi.id
          draft.value.activeCommandResponse = { type: 'LOADED', value: '' }
          draft.value.activeCommandCode = ''
          draft.value.commands.unshift(cmdFromApi)
        }) 
      })    
  }
  
  function executeCommand() {
    if(state.type !== 'LOADED' || !activeCommand)
      throw new Error()
    updateLoadedState(draft => {
      draft.activeCommandResponse = {
        type : 'LOADING'
      }
    })
    api.executeCsCommand(activeCommand.code)
      .then(r => {
        updateLoadedState(draft => {
          draft.activeCommandResponse = {
            type: 'LOADED',
            value: r.data
          }
        })
      })
      .catch(e => {
        let message: string
        
        if(e.response.status >= 400 && e.response.status < 500)
          message = `Bad command: ${e.response.data}`
        else if(e.response.status >= 500 && e.response.status < 600)
          message = `Server error: ${e.response.data}`
        
        updateLoadedState(draft => {
          draft.activeCommandResponse = {
            type: 'LOADED',
            value: message
          }
        })
      })
  }

  const [updateCommandDebounced] = useDebouncedCallback(updateCommand, 1000)

  if(state.type !== 'LOADED')
    return { type: state.type }

  return {
    type: 'LOADED',
    value: {
      activeCommand,
      activeCommandResponse: state.value.activeCommandResponse,
      commands: state.value.commands,
      setActiveCommand, 
      updateCommandCode, 
      updateCommandName, 
      addNewCommand, 
      executeCommand
    }
  }
}