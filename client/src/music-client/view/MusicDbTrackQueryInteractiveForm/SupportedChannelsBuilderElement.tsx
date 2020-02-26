import { IdWithName } from "../../../utils/types";
import { ElementBase } from "./ElementBase";
import { InputLabel, colors, makeStyles, TextField, Chip } from "@material-ui/core";
import { ems, percent } from "../../../utils/css";
import React from 'react'
import Autocomplete from '@material-ui/lab/Autocomplete';
import { difference } from 'lodash'

interface Props {
  label: string;
  availableChannels: IdWithName[]
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
  channelChip: {
    margin: ems(0.3, 0.3)
  }
}));

export const SupportedChannelsBuilderElement = (props: Props) => {  
  const styles = useStyles();

  const nameOf = (channelId: string) => props.availableChannels.find(c => c.id === channelId)!.name

  const unpickedChannels = difference(props.availableChannels.map(c => c.id), props.value)
    .map(cId => props.availableChannels.find(c => c.id == cId)!)

  const handlePickerChange = (channelId: string | null) => {
    if(channelId){
      props.onChange([ ...props.value, channelId ])
    }
  }

  const removeChannel = (channelId: string) => {
    const withoutIt = props.value.filter(cId => cId != channelId)
    props.onChange(withoutIt)
  }
  
  return (
    <ElementBase onRemove={props.onRemove} className={styles.base}>
      <InputLabel className={styles.label}>{props.label}</InputLabel>
      <div>
        {props.value.map(channelId => (
          <Chip className={styles.channelChip} size='small' label={nameOf(channelId)} onDelete={() => removeChannel(channelId)} />
        ))}
      </div>
      <Autocomplete 
        options={unpickedChannels} 
        getOptionLabel={o => o.name} 
        renderInput={params => <TextField {...params} />}
        onChange={(event: any, value: IdWithName | null) => handlePickerChange(value && value.id)}
      />
    </ElementBase>
  );
}