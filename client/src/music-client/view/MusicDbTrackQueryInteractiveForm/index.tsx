import React from "react";
import { ChipListElement } from "./ChipListElement";
import { InlineRangeElement } from "./InlineRangeElement";
import AddIcon from "@material-ui/icons/Add";
import { Menu, MenuItem, Fab } from "@material-ui/core";
import { makeStyles } from "@material-ui/styles";
import { ems, percent } from "../../../utils/css";
import { TrackQueryForm } from "../../shared";
const { bindTrigger, bindMenu } = require("material-ui-popup-state/hooks");

export interface MusicDbTrackQueryInteractiveFormProps {
  className?: string;
  input: TrackQueryForm;
  onChange: (i: TrackQueryForm) => void;
}

const useStyles = makeStyles(() => ({
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

const mapFieldValueToName = (field: string) => {
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

export const MusicDbTrackQueryInteractiveForm = (p: MusicDbTrackQueryInteractiveFormProps) => {

  const classes = useStyles();

  // const maybeRenderChipListElement = (
  //   name: Field,
  //   label: string,
  //   availableOptions: string[],
  //   selectedOptions: string[]
  // ) =>
  //   isFieldActive(name) && (
  //     <ChipListElement
  //       label={label}
  //       availableOptions={availableOptions}
  //       value={selectedOptions}
  //       onChange={onPropChange(name)}
  //       onRemove={() => setFieldInactive(name)}
  //     />
  //   );

  return (
    <div className={p.className}>
      {/* {maybeRenderChipListElement(
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
      )} */}
      {/* <Fab    
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
      </Menu> */}
    </div>
  );
};
