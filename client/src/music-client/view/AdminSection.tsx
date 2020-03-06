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
    marginLeft: ems(3),
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

  const { commands, activeCommand, setActiveCommandName, responseYaml } = useAdminSectionLogic()
  
  const styles = useStyles()

  return (
    <div className={styles.root}>
      <div className={styles.commandEditorCol}>
        <div className={styles.commandEditorActionBar}>
          <Select
            value={activeCommand.name} 
            onChange={e => setActiveCommandName(e.target.value as string)} 
            className={styles.commandSelect}
          >
            {commands.map(commandName => (
              <MenuItem key={commandName} value={commandName}>{commandName}</MenuItem>
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
          value={activeCommand.yaml}
        />
        <Button variant="contained" className={styles.executeButton}>Execute</Button>
      </div>
      <YamlEditor
        className={styles.response}
        codeMirrorRootClassName={styles.codeMirrorRoot}
        value={responseYaml}
      />
    </div>
  )
}