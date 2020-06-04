import { RemovableElementBase } from "./RemovableElementBase"
import { TextField as Input, makeStyles, InputLabel, colors } from '@material-ui/core'
import React from 'react'
import { percent, ems } from "../../../utils/css"

interface TextFieldProps {
  label: string
  value: string
  onChange: (value: string) => void
  onRemove: () => void
}

const useStyles = makeStyles({
  base: {
    width: percent(100),
    padding: ems(0.7, 0.7, 0.8),
    position: "relative"
  },
  label: {
    color: colors.grey[800],
    fontSize: ems(1.2),
    marginLeft: percent(12),
    marginBottom: ems(0.3)
  },
  field: {
    width: percent(100)
  },
}, { name: 'TextField' })

export const TextField = (props: TextFieldProps) => {
  const styles = useStyles()
  return (
    <RemovableElementBase className={styles.base} onRemove={props.onRemove}>
      <InputLabel className={styles.label}>{props.label}</InputLabel>
      <Input
        className={styles.field}
        value={props.value}
        onChange={e => props.onChange(e.target.value)}
      />
    </RemovableElementBase>
  )
}