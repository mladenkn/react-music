import React from 'react';
import { makeStyles, Button, TextField, Snackbar } from '@material-ui/core';
import { ems } from '../../utils/css';
import { YamlEditor } from '../../utils/view/YamlEditor';
import { useAdminSectionLogic } from '../logic/adminSection';
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline';
import EditIcon from '@material-ui/icons/Edit';
import BackupIcon from '@material-ui/icons/Backup';
import { IconButtonWithTextFieldPopup } from '../../utils/view/IconButtonWithTextFieldPopup';
import { Autocomplete } from '@material-ui/lab';
import { AdminCommand } from '../shared/admin';

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
    height: ems(12)
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
    return (
      <div className={styles.root}>
        <div className={styles.commandEditorCol}>
          <div className={styles.commandEditorActionBar}>
            <Autocomplete
              value={logic.data.activeCommand} 
              className={styles.commandSelect}
              options={logic.data.commands}
              getOptionLabel={o => o.name}
              renderInput={params => <TextField {...params} />}
              onChange={(event: any, command: AdminCommand | null) => command && logic.data.setActiveCommand(command.id)} 
            />
            <div className={styles.commandEditorActionBarRight}>
              <IconButtonWithTextFieldPopup 
                textFieldInitialValue={logic.data.activeCommand.name}
                onCommit={logic.data.updateCommandName}
                popupId={1} 
                size='small'
              >
                <EditIcon />
              </IconButtonWithTextFieldPopup>
              <IconButtonWithTextFieldPopup 
                onCommit={logic.data.addNewCommand}
                popupId={2} 
                className={styles.addButton} 
                size='small'
              >
                <AddCircleOutlineIcon />
              </IconButtonWithTextFieldPopup>
            </div>          
          </div>
          <YamlEditor
            className={styles.command}
            codeMirrorRootClassName={styles.commandCodeMirrorRoot}
            value={{ type: 'LOADED', data: logic.data.activeCommand.yaml }}
            onChange={logic.data.updateCommandYaml}
          />
          <YamlEditor 
            className={styles.jsMapper}
            codeMirrorRootClassName={styles.jsMapperCodeMirrorRoot} 
            value={{ type: 'LOADED', data: logic.data.jsMapperYaml }}
            onChange={logic.data.updateJsMapperYaml}
          />
          <Button onClick={logic.data.executeCommand} variant="contained" className={styles.executeButton}>Execute</Button>
        </div>
        <div className={styles.responseColumn}>
          <YamlEditor
            className={styles.response}
            codeMirrorRootClassName={styles.responseCodeMirrorRoot}
            value={logic.data.activeCommandResponseYaml}
          />
          <IconButtonWithTextFieldPopup
            className={styles.backupButton}
            textFieldInitialValue={() => ' - ' + new Date().toLocaleString()}
            onCommit={logic.data.backupResponse}
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
          open={logic.data.backupCreatedMessageShown}
          autoHideDuration={6000}
          onClose={logic.data.hideBackupCreatedMessage}
          message="Backup created"
        />
      </div>
    )
  }
  else 
    return <div>Error</div>
}