import { Loadable, Loaded } from "../../utils/types";
import { CsCommand } from "../shared/admin";
import { useImmer } from "use-immer";
import { useAdminApi } from "../api/adminApi";
import yaml from 'js-yaml';

interface CsCommandsState {
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

  const [state, updateState] = useImmer<Loadable<CsCommandsState>>({
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

  if(state.type !== 'LOADED')
    throw new Error()

  const activeCommand = {
    id: 1,
    name: 'First command',
    code: state.data.activeCommandCode
  }

  function setActiveCommand(cmdId: number) {

  }
  
  function updateCommandCode(code: string) {
    updateState(draft => {
      const draft_ = draft as Loaded<CsCommandsState>
      draft_.data.activeCommandCode = code
    })
  }
  
  function updateCommandName(newName: string) {

  }
  
  function addNewCommand(name: string) {

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