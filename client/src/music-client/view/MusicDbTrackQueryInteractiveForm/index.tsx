import React from "react";
import { ChipListElement } from "./ChipListElement";
import { InlineRangeElement } from "./InlineRangeElement";
import AddIcon from "@material-ui/icons/Add";
import { Menu, MenuItem, Fab, TextField } from "@material-ui/core";
import { makeStyles } from "@material-ui/styles";
import { ems, percent } from "../../../utils/css";
import { MusicDbTrackQueryForm } from "../../shared";
import { useMusicDbTrackQueryFormLogic } from "../../logic/musicDbtrackQueryForm";
import { ElementBase } from "./ElementBase";
import classes from "*.module.css";
import clsx from "clsx";
const { bindTrigger, bindMenu, usePopupState } = require("material-ui-popup-state/hooks");

export interface MusicDbTrackQueryInteractiveFormProps {
  className?: string;
  input: MusicDbTrackQueryForm;
  onChange: (i: MusicDbTrackQueryForm) => void;
}

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex'
  },
  year: {
    width: percent(100)
  },
  titleContainsField: {
    padding: ems(0.5 , 0.6)
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
}), {name: 'MusicDbTrackQueryInteractiveForm'});

type Field = keyof MusicDbTrackQueryForm

const mapFieldValueToName = (field: Field) => {
  switch (field) {
    case "youtubeChannelId":
      return "Channel";
    case "mustHaveEveryTag":
      return "Must have all tags";
    case "mustHaveAnyTag":
      return "Must have some tags";
    case "titleContains":
      return "Title Contains";
    case "yearRange":
      return "Year";
  }
}

export const MusicDbTrackQueryInteractiveForm = (p: MusicDbTrackQueryInteractiveFormProps) => {

  const styles = useStyles();
  const { 
    values, 
    availableTags, 
    inactiveFields, 
    isFieldActive, 
    onFieldChange, 
    setFieldActive,
    setFieldInactive 
  } = useMusicDbTrackQueryFormLogic({
    values: p.input,
    onChange: p.onChange
  })
  const popupState = usePopupState({ variant: "popover", popupId: "TrackQueryInteractiveFormPopup" })

  return (
    <div className={clsx(p.className, styles.root)}>
      <div>      
        {isFieldActive('mustHaveEveryTag') && (
            <ChipListElement
              label='Must have all tags'
              availableOptions={availableTags}
              value={values.mustHaveEveryTag}
              onChange={onFieldChange('mustHaveEveryTag')}
              onRemove={() => setFieldInactive('mustHaveEveryTag')}
            />
          )}

        {isFieldActive('mustHaveAnyTag') && (
          <ChipListElement
            label='Must have some tags'
            availableOptions={availableTags}
            value={values.mustHaveAnyTag}
            onChange={onFieldChange('mustHaveAnyTag')}
            onRemove={() => setFieldInactive('mustHaveAnyTag')}
          />
        )}

        {isFieldActive("yearRange") && (
          <InlineRangeElement
            className={styles.year}
            label='Year'
            value={values.yearRange!}
            onChange={onFieldChange("yearRange")}
            onRemove={() => setFieldInactive("yearRange")}
          />
        )}

        {isFieldActive('titleContains') &&
          <ElementBase className={styles.titleContainsField} onRemove={() => setFieldInactive('titleContains')}>
            <TextField 
              label='Title contains'
              value={values.titleContains}
              onChange={onFieldChange("titleContains")}          
            />
          </ElementBase>
        }
      </div>
      <Fab 
        className={styles.addPropertyButton}
        size='small'
        color="primary"
        aria-label="add"
        {...bindTrigger(popupState)}
      >
        <AddIcon className={styles.addPropertyButtonIcon} />
      </Fab>
      <Menu {...bindMenu(popupState)}>
        {inactiveFields.map((f, i) => (
          <MenuItem key={i} dense onClick={() => setFieldActive(f)}>
            {mapFieldValueToName(f)}
          </MenuItem>
        ))}
      </Menu>
    </div>
  );
};