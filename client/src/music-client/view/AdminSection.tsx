import React, { useState } from 'react';
import { makeStyles, Select, MenuItem, Button } from '@material-ui/core';
import { ems } from '../../utils/css';
import { YamlEditor } from '../../utils/view/YamlEditor';
import { useAdminSectionLogic } from '../logic/adminSection';
const useStyles = makeStyles({
  root: {
    display: 'flex',
    alignItems: 'start',
  },
  querySelect: {
    marginTop: ems(2),
    width: ems(10),
  },
  queryEditorCol: {
    width: ems(30),
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
    width: ems(30),
    marginLeft: ems(1)
  },
  codeMirrorRoot: {
    height: ems(30)
  }
}, { name: 'AdminSection' })

export const AdminSection = () => {

  const { queries, activeQuery, setActiveQueryName, responseYaml } = useAdminSectionLogic()
  
  const styles = useStyles()

  return (
    <div className={styles.root}>
      <Select 
        value={activeQuery.name} 
        onChange={e => setActiveQueryName(e.target.value as string)} 
        className={styles.querySelect}
      >
        {queries.map((queryName) => (
          <MenuItem key={queryName} value={queryName}>{queryName}</MenuItem>
        ))}
      </Select>
      <div className={styles.queryEditorCol}>
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