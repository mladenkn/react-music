import React from 'react'
import { createStyles, withStyles, WithStyles } from '@material-ui/styles'
import { Chip } from '@material-ui/core'
import { ems } from '../css'

const styles = createStyles({
  chip: {
    marginRight: ems(0.5),
    marginBottom: ems(0.5),
  },
})

interface Props extends WithStyles<typeof styles> {
  className?: string
  availableOptions: string[]
  selectedOptions: string[]
  onChange: (selectedOptions: string[]) => void
} 

const ChipListPresenter = (p: Props) => {

  const options = p.availableOptions.map(o => {
    const isSelected = p.selectedOptions.includes(o)
    return { value: o, isSelected }
  })

  const handleChipClick = (value: string) => {
    const { isSelected } = options.find(o => o.value === value)!
    if(isSelected)
      p.onChange(p.selectedOptions.filter(v => v !== value))
    else
      p.onChange([...p.selectedOptions, value])
  }

  return (
    <div className={p.className}>
      {options.map(o => 
        <Chip 
          key={o.value}
          className={p.classes.chip} 
          size='small'
          label={o.value} 
          color={o.isSelected ? 'primary' : 'default'}
          onClick={() => handleChipClick(o.value)} 
        />
      )}
    </div>
  )
}

export const ChipList = withStyles(styles)(ChipListPresenter)