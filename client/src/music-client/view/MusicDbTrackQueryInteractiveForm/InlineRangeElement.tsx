import React from "react";
import { makeStyles } from "@material-ui/styles";
import { ems } from "../../../utils/css";
import { colors, InputLabel, Input } from "@material-ui/core";
import { ElementBase } from "./ElementBase";
import clsx from "clsx";

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

interface Value {
  from?: number;
  to?: number;
}

interface Props {
  className?: string;
  label: string;
  value?: Value;
  onChange: (value: Value) => void;
  onRemove: () => void
}

export const InlineRangeElement = (props: Props) => {
  const classes = useStyles();
  
  const onPropChange = (name: "from" | "to") => (event: {target: {value: string}}) => {
    const value = parseInt(event.target.value)
    const newValue = { ...props.value, [name]: value };
    props.onChange(newValue);
  };

  return (
    <ElementBase className={clsx(classes.base, props.className)} onRemove={props.onRemove}>
      <>
        <InputLabel className={classes.label}>{props.label}</InputLabel>
        <Input
          className={classes.lowerBound}
          classes={{
            input: classes.input
          }}
          type="number"
          value={props.value && props.value.from!}
          onChange={onPropChange("from")}
        />
        --
        <Input        
          className={classes.upperBound}
          classes={{
            input: classes.input
          }}
          type="number"
          value={props.value && props.value.to!}
          onChange={onPropChange("to")}
        />
      </>
    </ElementBase>
  );
};