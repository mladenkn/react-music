import React from "react";
import { makeStyles } from "@material-ui/styles";
import { ems } from "../../../utils/css";
import { colors, InputLabel, Input } from "@material-ui/core";
import { RemovableElementBase } from "./RemovableElementBase";
import clsx from "clsx";
import { Range } from '../../shared'

const useStyles = makeStyles(() => ({
  base: {
    display: "flex",
    alignItems: "center",
    padding: ems(0.5, 1),
    position: "relative",
    width: ems(19)
  },
  label: {
    color: colors.grey[800],
    fontSize: ems(1.2),
    marginTop: ems(0.5)
  },
  lowerBound: {
    marginLeft: ems(1),
    marginRight: ems(0.7),
    width: ems(4.3)
  },
  upperBound: {
    marginLeft: ems(0.7),
    width: ems(4.3)
  },
  input: {
    padding: ems(0.2, 0.4),
  }
}));

interface Props {
  className?: string;
  label: string;
  value?: Partial<Range<number>>;
  onChange: (value: Partial<Range<number>>) => void;
  onRemove: () => void
}

export const InlineRangeElement = (props: Props) => {
  const classes = useStyles();
  
  const onPropChange = (name: keyof Range<number>) => (event: {target: {value: string}}) => {
    const value = parseInt(event.target.value)
    const newValue = { ...props.value, [name]: value || undefined };
    props.onChange(newValue);
  };
 
  return (
    <RemovableElementBase className={clsx(classes.base, props.className)} onRemove={props.onRemove}>
      <>
        <InputLabel className={classes.label}>{props.label}</InputLabel>
        <Input
          className={classes.lowerBound}
          classes={{
            input: classes.input
          }}
          type="number"
          value={props.value?.lowerBound!}
          onChange={onPropChange("lowerBound")}
        />
        --
        <Input        
          className={classes.upperBound}
          classes={{
            input: classes.input
          }}
          type="number"
          value={props.value?.upperBound!}
          onChange={onPropChange("upperBound")}
        />
      </>
    </RemovableElementBase>
  );
};