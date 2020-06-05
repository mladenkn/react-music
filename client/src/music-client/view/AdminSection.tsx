import React from 'react';
import { makeStyles, Select, MenuItem, Button } from '@material-ui/core';
import { ems, percent } from '../../utils/css';
import { YamlEditor } from '../../utils/view/YamlEditor';
import { useAdminSectionLogic } from '../logic/adminSection';
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline';
import EditIcon from '@material-ui/icons/Edit';
import { IconButtonWithTextFieldPopup } from '../../utils/view/IconButtonWithTextFieldPopup';

const useStyles = makeStyles({
  root: {
    display: 'flex',
    alignItems: 'start',
  },
  commandSelect: {
    minWidth: ems(10),
    maxWidth: percent(100)
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
    height: ems(43.7)
  },
  jsMapper: {
    width: ems(35),
    marginTop: ems(0.5),
  },
  jsMapperCodeMirrorRoot: {
    height: ems(27)
  },
}, { name: 'AdminSection' })

export const AdminSection = () => {

  const logic = useAdminSectionLogic()  
  const styles = useStyles()

  console.log(logic)

  if(logic.type === 'LOADING'){
    return <div>Loading...</div>
  }
  else if (logic.type === 'LOADED') {
    return (
      <div className={styles.root}>
        <div className={styles.commandEditorCol}>
          <div className={styles.commandEditorActionBar}>
            <Select
              value={logic.data.activeCommand.id} 
              onChange={e => logic.data.setActiveCommand(e.target.value as number)} 
              className={styles.commandSelect}
            >
              {logic.data.commands.map(command => (
                <MenuItem key={command.id} value={command.id}>{command.name}</MenuItem>
              ))}
            </Select>
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
        <YamlEditor
          className={styles.response}
          codeMirrorRootClassName={styles.responseCodeMirrorRoot}
          value={logic.data.activeCommandResponseYaml}
        />
      </div>
    )
  }
  else 
    return <div>Error</div>
}