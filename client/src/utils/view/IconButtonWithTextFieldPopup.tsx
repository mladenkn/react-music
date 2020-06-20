import React from 'react'
import { IconButton, Popover, TextField, makeStyles } from "@material-ui/core"
import { bindPopover, bindTrigger } from "material-ui-popup-state/core"
import { ComponentProps } from "react"
import { usePopupState } from "material-ui-popup-state/hooks"
import { ems } from '../css'
import { omit } from 'lodash'

const useStyles = makeStyles({
  textField: {
    padding: ems(0.4, 0)
  }
}, { name: 'IconButtonWithTextFieldPopupProps' })

interface IconButtonWithTextFieldPopupProps extends ComponentProps<typeof IconButton> {
  popupId: number
  textFieldInitialValue?: string | (() => string)
  onCommit(text: string): void
}

// Napravit generičniju render props komponentu With{Sth}OnPopup
export const IconButtonWithTextFieldPopup = (props: IconButtonWithTextFieldPopupProps) => {
  const popupId = 'IconButtonWithTextFieldPopup-popup-' + props.popupId
  const popupState = usePopupState({ variant: "popover", popupId })
  
  const styles = useStyles()

  const handleKeyDown = (event: any) => {
    if(event.key === 'Enter'){
      props.onCommit(event.target.value)
      popupState.close()
    }
  }

  const buttonProps = omit(props, ['popupId', 'onCommit', 'textFieldInitialValue'])

  const getTextFieldInitialValue = () => {
    if(!props.textFieldInitialValue)
      return props.textFieldInitialValue;
    return typeof props.textFieldInitialValue === 'string' ? 
      props.textFieldInitialValue : 
      props.textFieldInitialValue()
  }
  
  return (
    <>
      <IconButton {...bindTrigger(popupState)} {...buttonProps} />
      <Popover
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
        {...bindPopover(popupState)}
      >
        <TextField defaultValue={getTextFieldInitialValue()} autoFocus onKeyDown={handleKeyDown} className={styles.textField} />
      </Popover>
    </>
  )
} 