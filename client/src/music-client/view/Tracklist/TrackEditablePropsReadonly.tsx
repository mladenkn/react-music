import { makeStyles } from "@material-ui/styles"
import { ems } from "../../../utils/css"
import clsx from "clsx"
import { Chip, Typography } from "@material-ui/core";
import React from 'react'
import { TrackDataEditableProps } from "../../logic/homeView";

const useItemEditablePropsReadonlyStyles = makeStyles(() => ({
	root: {

	},
  chipListProp: {
    display: 'flex',
    alignItems: 'baseline',
  },
  chipListPropValue: {
    display: 'flex',
    flexWrap: 'wrap',
  },
  chipListPropChip: {
    height: ems(2),
    margin: ems(0.2),
  },
  chipLabel: {
    padding: ems(0.7)
  },
  prop: {
    margin: ems(0.5, 0)
  },
}), {name: 'ItemEditablePropsReadonly'})

interface Props {
  className?: string
  track: TrackDataEditableProps
  textClassName?: string
}

export const TrackEditablePropsReadonlyUI = (p: Props) => {
  const classes = useItemEditablePropsReadonlyStyles()
  const { year, tags } = p.track
	return (
		<div className={clsx(classes.root, p.className)}>
      {tags.length > 0 &&
        <div className={clsx(classes.chipListProp, classes.prop)}>
          <Typography className={p.textClassName}>Tags:</Typography>
          <div className={classes.chipListPropValue}>
            {tags!.map(v => (        
              <Chip 
                key={v} 
                classes={{label: classes.chipLabel}} 
                className={clsx(classes.chipListPropChip, p.textClassName)} 
                label={v} 
              />
            ))}
          </div>
        </div>
      }
      {year && <Typography className={clsx(p.textClassName, classes.prop)}>Year: {year}</Typography>}
		</div>
	)
}
