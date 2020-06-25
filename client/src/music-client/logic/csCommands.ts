import { Loadable } from "../../utils/types";
import { CsCommand } from "../shared/admin";
import { useImmer } from "use-immer";
import { useAdminApi } from "../api/adminApi";
import { useDebouncedCallback } from "use-debounce/lib";
import { useEffect } from "../../utils/useEffect";

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

  function updateCommand(cmd: CsCommand) {
    api.updateCommand(cmd)
      .then(() => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          const cmd_ = draft.value.commands.find(q => q.id === draft.value.activeCommandId)!
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
    api.executeCsCommand(activeCommand.code)
      .then(r => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          draft.value.activeCommandResponse = {
            type: 'LOADED',
            value: r.data
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