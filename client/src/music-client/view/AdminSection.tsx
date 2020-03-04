import React from 'react';
import { makeStyles } from '@material-ui/core';
import { ems } from '../../utils/css';
import { YamlEditor } from '../../utils/view/YamlEditor';

const useStyles = makeStyles({
  root: {
    display: 'flex'
  },
  queryEditor: {
    width: ems(40)
  },
  response: {
    width: ems(40),
    marginLeft: ems(3)
  },
  codeMirrorRoot: {
    height: ems(30)
  }
}, { name: 'AdminSection' })

export const AdminSection = () => {

  const styles = useStyles()

  const yaml = `name: Martin D'vloper
job: Developer
skill: Elite
employed: True
foods:
    - Apple
    - Orange
    - Strawberry
    - Mango
languages:
    perl: Elite
    python: Elite
    pascal: Lame
education: |
    4 GCSEs
    3 A-Levels
    BSc in the Internet of Things 
`;

  return (
    <div className={styles.root}>      
      <YamlEditor
        className={styles.queryEditor}
        codeMirrorRootClassName={styles.codeMirrorRoot}
        value={yaml}
      />
      <YamlEditor
        className={styles.response}
        codeMirrorRootClassName={styles.codeMirrorRoot}
        value={''}
      />
    </div>
  )
}