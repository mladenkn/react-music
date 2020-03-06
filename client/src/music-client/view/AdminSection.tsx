import React, { useState } from 'react';
import { makeStyles, Select, MenuItem, Button, IconButton } from '@material-ui/core';
import { ems } from '../../utils/css';
import { YamlEditor } from '../../utils/view/YamlEditor';
import { useAdminSectionLogic } from '../logic/adminSection';
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline';
import EditIcon from '@material-ui/icons/Edit';

const useStyles = makeStyles({
  root: {
    display: 'flex',
    alignItems: 'start',
  },
  commandSelect: {
    width: ems(10),
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
              value={logic.data.activeCommand.name} 
              onChange={e => logic.data.setActiveCommand(e.target.value as string)} 
              className={styles.commandSelect}
            >
              {logic.data.commands.map(command => (
                <MenuItem key={command.name} value={command.name}>{command.name}</MenuItem>
              ))}
            </Select>
            <div className={styles.commandEditorActionBarRight}>
              <IconButton size='small'>
                <EditIcon />
              </IconButton>
              <IconButton className={styles.addButton} size='small'>
                <AddCircleOutlineIcon />
              </IconButton>
            </div>          
          </div>
          <YamlEditor
            codeMirrorRootClassName={styles.codeMirrorRoot}
            value={logic.data.activeCommand.yaml}
            onChange={logic.data.updateCommandYaml}
          />
          <Button variant="contained" className={styles.executeButton}>Execute</Button>
        </div>
        {logic.data.activeCommandResponseYaml.type === 'LOADED' ?
          <YamlEditor
            className={styles.response}
            codeMirrorRootClassName={styles.codeMirrorRoot}
            value={logic.data.activeCommandResponseYaml.data}
          /> :
          <YamlEditor 
            className={styles.response}
            codeMirrorRootClassName={styles.codeMirrorRoot} 
          />
        }  
      </div>
    )
  }
  else 
    return <div>Error</div>
}