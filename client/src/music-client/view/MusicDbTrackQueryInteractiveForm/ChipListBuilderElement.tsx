import { IdWithName } from "../../../utils/types";
import { RemovableElementBase } from "./RemovableElementBase";
import { InputLabel, colors, makeStyles, TextField, Chip } from "@material-ui/core";
import { ems, percent } from "../../../utils/css";
import React from 'react'
import { Autocomplete } from '@material-ui/lab';
import { difference } from 'lodash'

interface Props {
  label: string;
  availableItems: IdWithName[]
  value: string[]
  onChange(value: string[]): void
  onRemove: () => void
}

const useStyles = makeStyles(() => ({
  base: {
    width: percent(100),
    padding: ems(0.7, 0.7, 0.4),
    position: "relative"
  },
  label: {
    color: colors.grey[800],
    fontSize: ems(1.2),
    marginLeft: percent(12),
    marginBottom: ems(0.8)
  },
  itemChip: {
    margin: ems(0.3, 0.3)
  },
  picker: {
    width: percent(95),
    margin: '0.3em auto 0.3em'
  }
}));

export const ChipListBuilderElement = (props: Props) => {  
  const styles = useStyles();

  const getLabelOf = (itemId: string) => props.availableItems.find(c => c.id === itemId)!.name

  const unpickedItems = difference(props.availableItems.map(c => c.id), props.value)
    .map(cId => props.availableItems.find(c => c.id == cId)!)

  const handlePickerChange = (itemId: string) => props.onChange([ ...props.value, itemId ])

  const removeItem = (itemId: string) => {
    const withoutIt = props.value.filter(cId => cId != itemId)
    props.onChange(withoutIt)
  }
  
  return (
    <RemovableElementBase onRemove={props.onRemove} className={styles.base}>
      <InputLabel className={styles.label}>{props.label}</InputLabel>
      <div>
        {props.value.map(itemId => (
          <Chip className={styles.itemChip} size='small' label={getLabelOf(itemId)} onDelete={() => removeItem(itemId)} />
        ))}
      </div>
      <Autocomplete 
        className={styles.picker}
        options={unpickedItems} 
        getOptionLabel={o => o.name} 
        renderInput={params => <TextField {...params} />}
        onChange={(event: any, item: IdWithName | null) => item && handlePickerChange(item.id)}
      />
    </RemovableElementBase>
  );
}