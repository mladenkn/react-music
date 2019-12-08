import React, { useState } from 'react';
import { createStyles, withStyles, WithStyles, Theme, Dialog, DialogContent, DialogTitle, Input, DialogActions, Button } from "@material-ui/core";
import { ems, percent } from '../css';

type TextInputDialogProps = {
    heading?: string
    initialInput?: string
    isOpen: boolean
    isInputValid?: (input: string) => boolean
    onOK: (url: string) => void
    onCancel: () => void
} & WithStyles<typeof textInputDialogStyle>

export const textInputDialogStyle = ({breakpoints}: Theme) => createStyles({

  [breakpoints.only('xs')]: {
      paper: {
          //margin: ems(0, 10),
          width: percent(100),
      },
      input: {
          width: percent(100),
      },
  },

  [breakpoints.up('sm')]: {
      input: {
          width: 400,
      },
  },

  actions:  {
      padding: ems(1, 1, 0.5),
  },
})

export const TextInputDialog = withStyles(textInputDialogStyle)((p: TextInputDialogProps) => {

  const [input, setInput] = useState(p.initialInput || '')
  const inputValid = p.isInputValid || (() => true)
  const okButtonDisabled = !inputValid(input)

  return (
    <Dialog classes={{paper: p.classes.paper}} open={p.isOpen} aria-labelledby="text-input-dialog">
      <DialogTitle id="text-input-dialog">{p.heading}</DialogTitle>
  
      <DialogContent>
        <Input className={p.classes.input} value={input} onChange={e => setInput(e.target.value)} />
      </DialogContent>
  
      <DialogActions className={p.classes.actions}>
        <Button variant="contained" color="secondary" onClick={() => p.onCancel()} className={''}>
          Cancel
        </Button>
        <Button variant="contained" color="primary" onClick={() => p.onOK(input)} disabled={okButtonDisabled} autoFocus>
          OK
        </Button>        
      </DialogActions>
    </Dialog>
  )
})