import { IdWithName } from "../../../utils/types";
import { ElementBase } from "./ElementBase";
import { InputLabel, colors, makeStyles, TextField } from "@material-ui/core";
import { ems, percent } from "../../../utils/css";
import React from 'react'
import Autocomplete from '@material-ui/lab/Autocomplete';

interface Props {
  label: string;
  availableChannels: IdWithName[]
  value: string[]
  onChange(value: IdWithName[]): void
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
  removeButton: {
    padding: ems(0.2),
    position: "absolute",
    right: 0,
    top: 0
  },
  value: {
    marginTop: ems(0.5)
  }
}));

export const SupportedChannelsBuilderElement = (props: Props) => {  
  const classes = useStyles();
  return (
    <ElementBase onRemove={props.onRemove} className={classes.base}>
      <InputLabel className={classes.label}>{props.label}</InputLabel>
      <Autocomplete 
        options={props.availableChannels} 
        getOptionLabel={o => o.name} 
        renderInput={params => <TextField {...params} />}
        onChange={(event: any, value: IdWithName | null) => console.log(`picked new channel ${value && value.id}`)}
      />
    </ElementBase>
  );
}