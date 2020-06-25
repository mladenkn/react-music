import { useAdminApi } from "../api/adminApi"
import { Loadable } from "../../utils/types"
import { useImmer } from "use-immer"
import { CsCommandsLogic, useCsCommands } from "./csCommands"

interface State {
  savedToVariableMessageShown: boolean
}

interface AdminSectionLogic extends CsCommandsLogic {
  backupCreatedMessageShown: boolean
  saveResponseToVariable(key: string): void
  hideSavedToVariableMessage(): void
}

const initalCmdResponse = ''

export const useAdminSectionLogic = (): Loadable<AdminSectionLogic> => {

  const api = useAdminApi()
  const commands = useCsCommands()

  const [state, updateState] = useImmer<State>({
    savedToVariableMessageShown: false
  })
  
  function saveResponseToVariable(key: string){

  }
  
  function hideSavedToVariableMessage(){
    
  }

  if(commands.type === 'LOADED'){
    return {
      type: 'LOADED',
      data: {
        ...commands.data,
        backupCreatedMessageShown: false,
        saveResponseToVariable,
        hideSavedToVariableMessage
      }
    }
  }
  else 
    return { type: commands.type }
}