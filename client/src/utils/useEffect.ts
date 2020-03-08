import React, { DependencyList, useRef } from 'react';
import { EffectCallback } from 'react';

interface Options {
  runOnFirstRender: boolean
}

const defaultOptions = { runOnFirstRender: false }

export function useEffect(effect: EffectCallback, deps?: DependencyList, o: Options = defaultOptions){
  const isFirstRun = useRef(true)
  React.useEffect(() => {
    if(isFirstRun.current && !o.runOnFirstRender){
      isFirstRun.current = false
      return
    }
    effect()
  }, deps)
}