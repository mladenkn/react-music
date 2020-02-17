import { percent, ems } from "../../../utils/css";
import clsx from "clsx";
import { makeStyles, TextField } from "@material-ui/core";
import React from 'react'
import ChipInput from "../../../utils/view/ChipInput";
import { TrackEditableProps } from "../../shared/track";

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
  track: TrackEditableProps
  onChange(track: TrackEditableProps): void
}

export const TrackEditablePropsEditUI = (p: Props) => {
  const classes = useItemEditablePropsEditStyles()

  function onPropChange(key: keyof TrackEditableProps){
    p.onChange({...p.track})
  }

	return (
		<div className={clsx(classes.root, p.className)}>
      <ChipInput
        className={clsx(classes.chipList, classes.tags)}
        classes={{
          chip: classes.chipListPropChip,
        }}
        label='Tags'
        value={p.track.tags} 
        onChange={tags => {
          p.onChange({ tags, year: p.track.year })
        }}
      />
      <TextField 
        className={classes.year} 
        label='Year' 
        type='number' 
        value={p.track.year || ''} 
        onChange={e => {
          p.onChange({ tags: p.track.tags, year: parseInt(e.target.value) })
        }}
      />
		</div>
	)
}