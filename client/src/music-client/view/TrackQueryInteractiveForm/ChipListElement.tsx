import React from "react";
import { makeStyles } from "@material-ui/styles";
import { ems, percent } from "../../../utils/css";
import { colors, InputLabel } from "@material-ui/core";
import { ChipList } from "../../../utils/components/ChipList";
import { ElementBase } from "./ElementBase";

const useStyles = makeStyles(() => ({
  base: {
    marginBottom: ems(1),
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

interface Props {
  label: string;
  availableOptions: string[];
  value: string[];
  onChange: (value: string[]) => void;
  onRemove: () => void
}

export const ChipListElement = (p: Props) => {
  const classes = useStyles();
  return (
    <ElementBase onRemove={p.onRemove} className={classes.base}>
      <InputLabel className={classes.label}>{p.label}</InputLabel>
      <ChipList
        className={classes.value}
        availableOptions={p.availableOptions}
        selectedOptions={p.value}
        onChange={p.onChange}
      />
    </ElementBase>
  );
};
