import { Loadable } from "../../utils/types";
import { CsCommand } from "../shared/admin";
import { useImmer } from "use-immer";
import { useAdminApi } from "../api/adminApi";
import yaml from 'js-yaml';
import { useDebouncedCallback } from "use-debounce/lib";
import { useEffect } from "../../utils/useEffect";

interface State {
  activeCommandId: number
  activeCommandCode: string
  activeCommandResponseYaml: Loadable<string>
  commands: CsCommand[]  
}

export interface CsCommandsLogic {
  activeCommand: CsCommand
  activeCommandResponseYaml: Loadable<string>
  commands: CsCommand[]
  setActiveCommand(cmdId: number): void
  updateCommandCode(yaml: string): void
  updateCommandName(newName: string): void
  addNewCommand(name: string): void
  executeCommand(): void
}

export function useCsCommands(): Loadable<CsCommandsLogic> {

  const api = useAdminApi()

  const [state, updateState] = useImmer<Loadable<State>>({
    type: 'LOADED',
    data: {
      activeCommandCode: '',
      activeCommandId: 1,
      activeCommandResponseYaml: {
        type: 'LOADED',
        data: ''
      },
      commands: []
    }
  })

  useEffect(() => {
    api.getInitialParams()
      .then(response => {
        const activeCommandId = response.currentCommandId || response.commands[0].id
        updateState(() => ({
          type: 'LOADED',
          data: {
            activeCommandId,
            activeCommandCode: response.commands.find(c => c.id === activeCommandId)!.code,
            activeCommandResponseYaml: { type: 'LOADED', data: '' },
            commands: response.commands,
            jsMapperYaml: '',
            savedToVariableMessageShown: false,
          }
        }))
      })
  }, [], { runOnFirstRender: true })

  if(state.type !== 'LOADED')
    throw new Error()

  const activeCommand = {
    id: 1,
    name: 'First command',
    code: state.data.activeCommandCode
  }

  function updateCommand(cmd: CsCommand) {
    api.updateCommand(cmd)
      .then(() => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          const cmd = draft.data.commands.find(q => q.id === draft.data.activeCommandId)!
          cmd.name = cmd.name
          cmd.code = cmd.code
        })
      })
  }

  const [updateCommandDebounced] = useDebouncedCallback(updateCommand, 1000)

  function setActiveCommand(cmdId: number) {      
    updateState(draft => {
      if(draft.type !== 'LOADED')
        throw new Error()
      draft.data.activeCommandId = cmdId
      draft.data.activeCommandCode = draft.data.commands.find(c => c.id === cmdId)!.code
    })
  }
  
  function updateCommandCode(code: string) {
    updateState(draft => {
      if(draft.type !== 'LOADED')
        throw new Error()
      draft.data.activeCommandCode = code
    })
    updateCommandDebounced({ ...activeCommand, code })
  }
  
  function updateCommandName(newName: string) {
    if(state.type !== 'LOADED')
      throw new Error()
    updateCommand({ id: activeCommand.id, name: newName, code: state.data.activeCommandCode })
  }
  
  function addNewCommand(name: string) {
    const cmd = { name, code: '' }
    api.addCommand(cmd)
      .then((cmdFromApi) => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          draft.data.activeCommandId = cmdFromApi.id
          draft.data.activeCommandResponseYaml = { type: 'LOADED', data: '' }
          draft.data.activeCommandCode = ''
          draft.data.commands.unshift(cmdFromApi)
        }) 
      })    
  }
  
  function executeCommand() {
    if(state.type !== 'LOADED')
      throw new Error()
    api.executeCsCommand(state.data.activeCommandCode)
      .then(r => {
        updateState(draft => {
          if(draft.type !== 'LOADED')
            throw new Error()
          const yamlResponse = yaml.safeDump(r.data)
          draft.data.activeCommandResponseYaml = {
            type: 'LOADED',
            data: yamlResponse
          }
        })
      })
  }

  return {
    type: 'LOADED',
    data: {
      activeCommand,
      activeCommandResponseYaml: state.data.activeCommandResponseYaml,
      commands: [],
      setActiveCommand, 
      updateCommandCode, 
      updateCommandName, 
      addNewCommand, 
      executeCommand
    }
  }
}