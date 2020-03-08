import React, { useRef, useEffect, useState } from 'react';
import { Controlled } from 'react-codemirror2'
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/material.css';
import 'codemirror/mode/yaml/yaml';
import clsx from 'clsx';
import { makeStyles, CircularProgress } from '@material-ui/core';
import { percent } from '../css';
import { Loadable } from '../types';

const CodeMirror = Controlled as any

interface Props {
  className?: string
  value: Loadable<string>
  codeMirrorRootClassName?: string
  onChange?: (value: string) => void
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

  const handleChange = (value: string) => {
    console.log(value)
    props.onChange && props.onChange(value)
  }

  const content = props.value.type === 'LOADED' ? props.value.data : ''
 
  return (
    <>
      <CodeMirror
        ref={wrapped}
        className={clsx(props.className, !isReady && styles.rootHidden)}
        value={content}
        options={{
          mode: 'yaml',
          theme: 'material',
        }}
        onBeforeChange={(_: unknown, __: unknown, value: string) => handleChange(value)}
      />
      {props.value.type === 'LOADING' &&
        <CircularProgress size={60} color="secondary" className={styles.loadingSpinner} />        
      }
    </>
  )
}