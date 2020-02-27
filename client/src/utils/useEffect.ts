import React, { DependencyList, useRef } from 'react';
import { EffectCallback } from 'react';

export function useEffect(effect: EffectCallback, deps?: DependencyList){
  const isFirstRun = useRef(true)
  React.useEffect(() => {
    if(isFirstRun.current){
      isFirstRun.current = false
      return
    }
    effect()
  }, deps)
}