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
          data: {
            activeCommandId,
            activeCommandCode: response.commands.find(c => c.id === activeCommandId)?.code,
            activeCommandResponse: { type: 'LOADED', data: '' },
            commands: response.commands,
            savedToVariableMessageShown: false,
          }
        }))
      })
  }, [], { runOnFirstRender: true })

  const activeCommand = 
    state.type === 'LOADED' && state.data.activeCommandId ?
    {
      id: state.data.activeCommandId,
      name: state.data.commands.find(c => c.id === state.data.activeCommandId)!.name,
      code: state.data.activeCommandCode!
    } :
    undefined

  function updateCommand(cmd: CsCommand) {
    api.updateCommand(cmd)
      .then(() => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          const cmd_ = draft.data.commands.find(q => q.id === draft.data.activeCommandId)!
          cmd_.name = cmd.name
          cmd_.code = cmd.code
        })
      })
  }

  function setActiveCommand(cmdId: number) {      
    updateState(draft => {
      if(draft.type !== 'LOADED')
        throw new Error()
      draft.data.activeCommandId = cmdId
      draft.data.activeCommandCode = draft.data.commands.find(c => c.id === cmdId)!.code
    })
  }
  
  function updateCommandCode(code: string) {
    if(state.type !== 'LOADED' || !activeCommand)
      throw new Error()
    updateState(draft => {
      if(draft.type !== 'LOADED' || !activeCommand)
        throw new Error()
      draft.data.activeCommandCode = code
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
          draft.data.activeCommandId = cmdFromApi.id
          draft.data.activeCommandResponse = { type: 'LOADED', data: '' }
          draft.data.activeCommandCode = ''
          draft.data.commands.unshift(cmdFromApi)
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
          draft.data.activeCommandResponse = {
            type: 'LOADED',
            data: r.data
          }
        })
      })
  }

  const [updateCommandDebounced] = useDebouncedCallback(updateCommand, 1000)

  if(state.type !== 'LOADED')
    return { type: state.type }

  return {
    type: 'LOADED',
    data: {
      activeCommand,
      activeCommandResponse: state.data.activeCommandResponse,
      commands: state.data.commands,
      setActiveCommand, 
      updateCommandCode, 
      updateCommandName, 
      addNewCommand, 
      executeCommand
    }
  }
}