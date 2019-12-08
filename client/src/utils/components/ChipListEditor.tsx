import { withStyles, WithStyles, createStyles, ClickAwayListener, InputLabel } from "@material-ui/core";
import { ems } from "../css";
import { useState } from "react";
import clsx from "clsx";
import ChipInput from "./ChipInput";
import React from 'react'

interface ChipListEditorProps extends WithStyles<typeof chipListEditorStyle> { 
  className?: string
  label?: string
  initialValue: string[]
  onChange: (value: string[]) => void  
  fullWidth?: boolean
}

const chipListEditorStyle = createStyles({
	root: {   
    
  },
  label: {
    marginBottom: ems(0.5),
    fontSize: ems(1),
  },
  chip: {

  },
})

export const ChipListEditor = withStyles(chipListEditorStyle)((p: ChipListEditorProps) =>
{
  const [focused, setFocused] = useState(false)
  return (
    <ClickAwayListener onClickAway={() => setFocused(false)}>
      <div onClick={() => setFocused(true)} className={clsx(p.classes.root, p.className)}>
        {/* <InputLabel focused={focused} className={p.classes.label}>{p.label}</InputLabel> */}
        <ChipInput
          label={p.label}
          classes={{chip: p.classes.chip}}
          fullWidth={p.fullWidth} 
          defaultValue={p.initialValue} 
          onChange={p.onChange} 
        />
      </div>
    </ClickAwayListener>
  )
})