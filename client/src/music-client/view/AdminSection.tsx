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
  response: {
    marginTop: ems(2.65),
    width: ems(35),
    marginLeft: ems(1),
  },
  codeMirrorRoot: {
    height: ems(40)
  },
  jsMapper: {
    width: ems(35),
    marginTop: ems(2.65),
    marginLeft: ems(1),
  } 
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
            codeMirrorRootClassName={styles.codeMirrorRoot}
            value={{ type: 'LOADED', data: logic.data.activeCommand.yaml }}
            onChange={logic.data.updateCommandYaml}
          />
        </div>
        <div>
          <YamlEditor 
            className={styles.jsMapper}
            codeMirrorRootClassName={styles.codeMirrorRoot} 
            value={{ type: 'LOADED', data: logic.data.jsMapperYaml }}
            onChange={logic.data.updateJsMapperYaml}
          />
          <Button onClick={logic.data.executeCommand} variant="contained" className={styles.executeButton}>Execute</Button>
        </div>
        <YamlEditor
          className={styles.response}
          codeMirrorRootClassName={styles.codeMirrorRoot}
          value={logic.data.activeCommandResponseYaml}
        />
      </div>
    )
  }
  else 
    return <div>Error</div>
}