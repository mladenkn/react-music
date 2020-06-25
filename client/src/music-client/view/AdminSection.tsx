import React from 'react';
import { makeStyles, Button, TextField, Snackbar, IconButton } from '@material-ui/core';
import { ems } from '../../utils/css';
import { CodeEditor } from '../../utils/view/YamlEditor';
import { useAdminSectionLogic } from '../logic/adminSection';
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline';
import EditIcon from '@material-ui/icons/Edit';
import BackupIcon from '@material-ui/icons/Backup';
import CloseIcon from '@material-ui/icons/Close';
import { IconButtonWithTextFieldPopup } from '../../utils/view/IconButtonWithTextFieldPopup';
import { Autocomplete } from '@material-ui/lab';
import { CsCommand } from '../shared/admin';
import yaml from 'js-yaml';
import { produce } from 'immer';
import { Loaded } from '../../utils/types';

const useStyles = makeStyles({
  root: {
    display: 'flex',
    alignItems: 'start',
  },
  commandSelect: {
    flex: 0.8,
  },
  commandEditorActionBar: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'baseline',
    marginBottom: ems(0.7),
  },
  commandEditorActionBarRight: {
    marginRight: ems(-0.3),
  },
  addButton: {
    marginLeft: ems(0.5)
  },
  commandEditorCol: {
    width: ems(35),
    display: 'flex',
    flexDirection: 'column'
  },
  executeButton: {
    marginRight: ems(0.5),
    marginTop: ems(1),
    alignSelf: 'flex-end',
  },
  command: {    
  },
  commandCodeMirrorRoot: {
    // height: ems(12)
  },
  response: {
    marginTop: ems(2.65),
    width: ems(36),
    marginLeft: ems(1),
  },
  responseCodeMirrorRoot: {
    height: ems(39.8)
  },
  jsMapper: {
    width: ems(35),
    marginTop: ems(0.5),
  },
  jsMapperCodeMirrorRoot: {
    height: ems(27)
  },
  responseColumn: {
    display: 'flex',
    flexDirection: 'column',
  },
  backupButton: {
    marginRight: ems(0.2),
    marginTop: ems(0.3),
    alignSelf: 'flex-end',
  },
}, { name: 'AdminSection' })

export const AdminSection = () => {

  const logic = useAdminSectionLogic()  
  const styles = useStyles()

  if(logic.type === 'LOADING'){
    return <div>Loading...</div>
  }
  else if (logic.type === 'LOADED') {
    const activeCommandResponse = logic.value.activeCommandResponse.type === 'LOADED' ?
      { type: 'LOADED', value: yaml.safeDump(logic.value.activeCommandResponse.value) } as Loaded<string> :
      logic.value.activeCommandResponse
    return (
      <div className={styles.root}>
        <div className={styles.commandEditorCol}>
          <div className={styles.commandEditorActionBar}>
            <Autocomplete
              value={logic.value.activeCommand} 
              className={styles.commandSelect}
              options={logic.value.commands}
              getOptionLabel={o => o.name}
              renderInput={params => <TextField {...params} />}
              onChange={(event: any, command: CsCommand | null) => command && logic.value.setActiveCommand(command.id)} 
            />
            <div className={styles.commandEditorActionBarRight}>
              {logic.value.activeCommand && <IconButtonWithTextFieldPopup 
                textFieldInitialValue={logic.value.activeCommand.name}
                onCommit={logic.value.updateCommandName}
                popupId={1} 
                size='small'
              >
                <EditIcon />
              </IconButtonWithTextFieldPopup>}
              <IconButtonWithTextFieldPopup 
                onCommit={logic.value.addNewCommand}
                popupId={2} 
                className={styles.addButton} 
                size='small'
              >
                <AddCircleOutlineIcon />
              </IconButtonWithTextFieldPopup>
            </div>          
          </div>
          <CodeEditor
            className={styles.command}
            codeMirrorRootClassName={styles.commandCodeMirrorRoot}
            value={{ type: 'LOADED', value: logic.value.activeCommand?.code || '' }}
            onChange={logic.value.activeCommand && logic.value.updateCommandCode}
            mode='clike'
          />
          {logic.value.activeCommand && <Button onClick={logic.value.executeCommand} variant="contained" className={styles.executeButton}>Execute</Button>}
        </div>
        <div className={styles.responseColumn}>
          <CodeEditor
            className={styles.response}
            codeMirrorRootClassName={styles.responseCodeMirrorRoot}
            value={activeCommandResponse}
            mode='yaml'
          />
          <IconButtonWithTextFieldPopup
            className={styles.backupButton}
            textFieldInitialValue={() => ' - ' + new Date().toLocaleString()}
            onCommit={logic.value.saveResponseToVariable}
            popupId={1}
          >
            <BackupIcon />
          </IconButtonWithTextFieldPopup>
        </div>
        <Snackbar
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'right',
          }}
          open={logic.value.backupCreatedMessageShown}
          autoHideDuration={6000}
          onClose={logic.value.hideSavedToVariableMessage}
          message="Variable set"
          action={
            <IconButton size="small" aria-label="close" color="secondary" onClick={logic.value.hideSavedToVariableMessage}>
              <CloseIcon fontSize="small" />
            </IconButton>
          }
        />
      </div>
    )
  }
  else 
    return <div>Error</div>
}