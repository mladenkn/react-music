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
  querySelect: {
    width: ems(10),
  },
  querEditorActionBar: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'baseline',
    marginBottom: ems(0.7),
  },
  querEditorActionBarRight: {
    marginRight: ems(-0.3),
  },
  addButton: {
    marginLeft: ems(0.5)
  },
  queryEditorCol: {
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

  const { queries, activeQuery, setActiveQueryName, responseYaml } = useAdminSectionLogic()
  
  const styles = useStyles()

  return (
    <div className={styles.root}>
      <div className={styles.queryEditorCol}>
        <div className={styles.querEditorActionBar}>
          <Select
            value={activeQuery.name} 
            onChange={e => setActiveQueryName(e.target.value as string)} 
            className={styles.querySelect}
          >
            {queries.map((queryName) => (
              <MenuItem key={queryName} value={queryName}>{queryName}</MenuItem>
            ))}
          </Select>
          <div className={styles.querEditorActionBarRight}>
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
          value={activeQuery.yaml}
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