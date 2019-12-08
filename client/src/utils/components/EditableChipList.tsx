import React from 'react'
import { Chip, IconButton, WithStyles, withStyles, createStyles } from "@material-ui/core";
import { ems, rgba } from "../css";
import clsx from "clsx";
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline'

const editableChipListStyles = createStyles({
	root: {
    display: 'flex',
    alignItems: 'center',
  },
  chip: {
    margin: ems(0, 0.2),
  },
  icon: {
    width: ems(0.75)
  },
  addChipButton: {
    padding: ems(0.15, 0.3),
  },
})

interface EditableChipListProps extends WithStyles<typeof editableChipListStyles> {
  className?: string
  values: string[]
} 

export const EditableChipListUI = withStyles(editableChipListStyles)((p: EditableChipListProps) => (
  <div className={clsx(p.classes.root, p.className)}>
    {p.values.map((v, i) => (
      <Chip 
        key={i}
        onDelete={() => { }} 
        classes={{ deleteIcon: p.classes.icon }} 
        className={p.classes.chip} 
        label={v} 
      />
    ))}
    <IconButton className={p.classes.addChipButton}>
      <AddCircleOutlineIcon className={p.classes.icon} />
    </IconButton>
  </div>
))

const chipEditStyles = createStyles({
  root: {
    color: rgba(0, 0, 0, 0.87),
    border: 'none',
    cursor: 'default',
    height: ems(3),
    display: 'inline-flex',
    outline: 'none',
    padding: '1em 1em 2.12em',
    fontSize: '0.8125em',
    boxSizing: 'border-box',
    transition: 'background-color 300ms cubic-bezier(0.4, 0, 0.2, 1) 0ms,box-shadow 300ms cubic-bezier(0.4, 0, 0.2, 1) 0ms',
    alignItems: 'center',
    fontFamily: "Roboto",
    whiteSpace: 'nowrap',
    borderRadius: ems(1.5),
    verticalAlign: 'middle',
    justifyContent: 'center',
    textDecoration: 'none',
    backgroundColor: '#e0e0e0',
  },
  input: {
    width: ems(5),
    fontSize: ems(1.15),
    backgroundColor: 'transparent',
    border: 0,
    borderBottom: '1px solid',
    marginTop: ems(0.9),
    outline: 'none',
    paddingBottom: ems(0.2),
  },
})
 
type ChipEditProps = {onConfirm: (input: string) => void} & WithStyles<typeof chipEditStyles>

export const ChipEditUI = withStyles(chipEditStyles)((p: ChipEditProps) => (
  <div className={p.classes.root}>
    <input className={p.classes.input} />
  </div>
))