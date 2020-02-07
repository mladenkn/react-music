import React from "react";
import { ChipListElement } from "./ChipListElement";
import { InlineRangeElement } from "./InlineRangeElement";
import AddIcon from "@material-ui/icons/Add";
import { Menu, MenuItem, Fab } from "@material-ui/core";
import { makeStyles } from "@material-ui/styles";
import clsx from "clsx";
import { ems, percent } from "../../../utils/css";
import { TrackDataForm, useRootLogic, Field } from "./rootLogic";
const { bindTrigger, bindMenu } = require("material-ui-popup-state/hooks");

export interface TrackQueryFromProps {
  className?: string;
  input: TrackDataForm;
  onChange: (i: TrackDataForm) => void;
}

const useStyles = makeStyles(() => ({
  root: {},
  year: {
    width: percent(100)
  },
  addPropertyButton: {
    float: "right",
    clear: "both",
    marginTop: ems(0.3),
    marginRight: ems(-0.5)
  },
  addPropertyButtonIcon: {
    width: ems(1.2),
    height: ems(1.2)
  }
}));

const mapFieldValueToName = (field: Field) => {
  switch (field) {
    case "channel":
      return "Channel";
    case "mustContainAllTags":
      return "Must have all tags";
    case "mustContainSomeTags":
      return "Must have some tags";
    case "titleMatch":
      return "Title";
    case "yearSpan":
      return "Year";
  }
};

export const TrackQueryInteractiveForm = (p: TrackQueryFromProps) => {
  const {
    form: { input, onPropChange },
    availableTags,
    popupState,
    inactiveFields,
    setFieldActive,
    setFieldInactive,
    isFieldActive
  } = useRootLogic(p);

  const classes = useStyles();

  const maybeRenderChipListElement = (
    name: Field,
    label: string,
    availableOptions: string[],
    selectedOptions: string[]
  ) =>
    isFieldActive(name) && (
      <ChipListElement
        label={label}
        availableOptions={availableOptions}
        value={selectedOptions}
        onChange={onPropChange(name)}
        onRemove={() => setFieldInactive(name)}
      />
    );

  return (
    <div className={clsx(p.className, classes.root)}>
      {maybeRenderChipListElement(
        "mustContainAllTags",
        "Must have all tags",
        availableTags,
        input.mustContainAllTags
      )}
      {maybeRenderChipListElement(
        "mustContainSomeTags",
        "Must have some tags",
        availableTags,
        input.mustContainSomeTags
      )}
      {isFieldActive("yearSpan") && (
        <InlineRangeElement
          className={classes.year}
          label={"Year"}
          value={input.yearSpan}
          onChange={onPropChange("yearSpan")}
          onRemove={() => setFieldInactive("yearSpan")}
        />
      )}
      <Fab    
        className={classes.addPropertyButton}
        size='small'
        color="primary"
        aria-label="add"
        {...bindTrigger(popupState)}
      >
        <AddIcon className={classes.addPropertyButtonIcon} />
      </Fab>
      <Menu {...bindMenu(popupState)}>
        {inactiveFields.map(f => (
          <MenuItem dense onClick={() => setFieldActive(f)}>
            {mapFieldValueToName(f)}
          </MenuItem>
        ))}
      </Menu>
    </div>
  );
};
