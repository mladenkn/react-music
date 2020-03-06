import React, { useRef, useEffect, useState } from 'react';
import {UnControlled} from 'react-codemirror2'
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/material.css';
import 'codemirror/mode/yaml/yaml';
import clsx from 'clsx';
import { makeStyles, CircularProgress } from '@material-ui/core';
import { percent } from '../css';

const CodeMirror = UnControlled as any

interface Props {
  className?: string
  value?: string
  codeMirrorRootClassName?: string
}

const useStyles = makeStyles({
  rootHidden: {
    visibility: 'hidden',
    position: 'relative'
  },
  loadingSpinner: {
    position: 'absolute',
    left: percent(43.5),
    top: percent(39),
  },
}, {name: 'YamlEditor'})

export const YamlEditor = (props: Props) => {
  const wrapped = useRef<any>()
  const isFirstRender = useRef(true)

  const [isReady, setIsReady] = useState(false)

  useEffect(() => {
    if(isFirstRender.current){      
      isFirstRender.current = false

      const codeMirror = wrapped.current.ref.getElementsByClassName('CodeMirror')[0] as HTMLElement
      props.codeMirrorRootClassName && codeMirror.classList.add(props.codeMirrorRootClassName)

      setIsReady(true)
    }
  })

  const styles = useStyles()
 
  return (
    <>
      <CodeMirror
        ref={wrapped}
        className={clsx(props.className, !isReady && styles.rootHidden)}
        value={props.value}
        options={{
          mode: 'yaml',
          theme: 'material',
        }}
      />
      {!props.value && <CircularProgress size={60} color="secondary" className={styles.loadingSpinner} />}
    </>
  )
}