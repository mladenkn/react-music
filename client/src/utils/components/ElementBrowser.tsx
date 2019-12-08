import { makeStyles, List, ListItem, colors } from "@material-ui/core"
import React, { useState } from 'react'

const useElementBrowserStyles = makeStyles(() => ({
    root: {
        display: 'flex'
    },
    elementSelector: {
        width: '20%',
        marginRight: '3%',
    }
}), {name: 'ElementBrowser'})

interface ElementBrowserProps {
    className?: string
    children: JSX.Element | JSX.Element[]
    consoleSwitch?: boolean
    legacySwitch? : boolean
}

const useElementBrowserState = (elements: JSX.Element | JSX.Element[]) => {
  const names = React.Children.map(elements, c => {
    return c.props.name || c.type.name
  })
  console.log(elements)
  const persistedSelectedName = localStorage.getItem('ElementBrowser-persistedSelectedName') 
  const [currentSelection, setCurrentSelection] = useState(persistedSelectedName || names[0])
    
  const setCurrentSelection_ = (name: string) => {
      localStorage.setItem('ElementBrowser-persistedSelectedName', name)
      setCurrentSelection(name)
  }    
  const onSelect = setCurrentSelection_
  
  const selectedElement = React.Children.toArray(elements)
    .find(c => (c.props.name || c.type.name).toLowerCase() === currentSelection.toLowerCase())
  
  return {currentSelection, onSelect, names, selectedElement}
}

export const ElementBrowserUI = (p: ElementBrowserProps) => {
    const classes = useElementBrowserStyles()
    const state = useElementBrowserState(p.children)

    if(p.consoleSwitch)
        (window as any).elementBrowser = { switchTo: state.onSelect, names: state.names }

    return (
        <div className={classes.root + ' ' + p.className}>
            {p.legacySwitch && <ElementSelectorUI className={classes.elementSelector} state={state} />}
            {state.selectedElement || 'Selected element not found'}
        </div>
    )
}

export const ElementBrowserElement = (p: {children: JSX.Element, name: string}) => p.children

const useElementSelectorStyles = makeStyles(() => ({
    root: {

    },
    selectedElement: {
        backgroundColor: colors.cyan[400]
    },
}), {name: 'ElementSelector'})

interface ElementSelectorProps {
    className?: string
    state: ReturnType<typeof useElementBrowserState>
}

export const ElementSelectorUI = (p: ElementSelectorProps) => {
    const classes = useElementSelectorStyles()
    return (
        <List className={classes.root + ' ' + p.className}>
            {p.state.names.map(n =>                 
                <ListItem 
                    button
                    onClick={() => p.state.onSelect(n)}
                    className={p.state.currentSelection === n ? classes.selectedElement : ''}
                >
                    {n}
                </ListItem>
            )}
        </List>
    )
}
