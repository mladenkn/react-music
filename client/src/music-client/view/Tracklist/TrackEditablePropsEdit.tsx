import { percent, ems } from "../../../utils/css";
import clsx from "clsx";
import { makeStyles, TextField } from "@material-ui/core";
import React from 'react'
import ChipInput from "../../../utils/view/ChipInput";
import { TrackEditableProps } from "../../shared";

const useItemEditablePropsEditStyles = makeStyles(() => ({
	root: {
	},
  chipList: {
    maxWidth: percent(90)
  },
  chipListPropChip: {
    height: ems(2),
  },
  year: {
    marginTop: ems(1)
  },
  tags: {
    marginTop: ems(0.4)    
  }
}), {name: 'ItemEditablePropsEdit'})

interface Props {
	className?: string
  textClassName?: string
}

export const TrackEditablePropsEditUI = (p: Props) => {
  const classes = useItemEditablePropsEditStyles()
  // const { input, onPropChange } = p.formLogic
	return (
		<div className={clsx(classes.root, p.className)}>
      {/* <ChipInput
        className={clsx(classes.chipList, classes.tags)}
        classes={{
          chip: classes.chipListPropChip,
        }}
        label='Tags'
        value={input.tags} 
        onChange={onPropChange('tags')}
      />
      <TextField 
        className={classes.year} 
        label='Year' 
        type='number' 
        value={input.year} 
        onChange={onPropChange('year')} 
      /> */}
		</div>
	)
}