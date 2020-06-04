import React from "react";
import { makeStyles } from "@material-ui/styles";
import { ems, percent } from "../../../utils/css";
import { colors, InputLabel } from "@material-ui/core";
import { ChipList } from "../../../utils/view/ChipList";
import { RemovableElementBase } from "./RemovableElementBase";

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
    <RemovableElementBase onRemove={p.onRemove} className={classes.base}>
      <InputLabel className={classes.label}>{p.label}</InputLabel>
      <ChipList
        className={classes.value}
        availableOptions={p.availableOptions}
        selectedOptions={p.value}
        onChange={p.onChange}
      />
    </RemovableElementBase>
  );
};
