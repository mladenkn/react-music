import React from 'react';
import { makeStyles } from '@material-ui/core';
import {UnControlled} from 'react-codemirror2'
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/material.css';
import { ems } from '../../utils/css';


const CodeMirror = UnControlled as any

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
}, { name: 'AdminSection' })

export const AdminSection = () => {

  const styles = useStyles()

  const json = `
  `;

  return (
    <div className={styles.root}>      
      <CodeMirror
        className={styles.queryEditor}
        value={json}
        options={{
          mode: 'json',
          theme: 'material',
        }}
        onChange={() => {
        }}
      />
      <CodeMirror
        className={styles.response}
        value={json}
        options={{
          mode: 'json',
          theme: 'material',
        }}
      />
    </div>
  )
} 