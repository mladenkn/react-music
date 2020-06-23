import { Loadable } from "../../utils/types";
import { CsCommand } from "../shared/admin";

interface CsCommandsState {
  activeCommandId: number
  activeCommandCode: string
  activeCommandResponseYaml: Loadable<string>
  commands: CsCommand[]  
}

interface CsCommandsLogic {
  activeCommand: CsCommand
  activeCommandResponseYaml: Loadable<string>
  commands: CsCommand[]
  setActiveCommand(cmdId: number): void
  updateCommandCode(yaml: string): void
  updateCommandName(newName: string): void
  addNewCommand(name: string): void
  executeCommand(): void
}

export function useCsCommands(): CsCommandsLogic {

  const activeCommand = {
    id: 1,
    name: 'First command',
    code: ''
  }

  function setActiveCommand(cmdId: number) {

  }
  
  function updateCommandCode(yaml: string) {

  }
  
  function updateCommandName(newName: string) {

  }
  
  function addNewCommand(name: string) {

  }
  
  function executeCommand() {

  }

  return {
    activeCommand,
    activeCommandResponseYaml: {type: 'LOADED', data: ''},
    commands: [],
    setActiveCommand, 
    updateCommandCode, 
    updateCommandName, 
    addNewCommand, 
    executeCommand
  }
}