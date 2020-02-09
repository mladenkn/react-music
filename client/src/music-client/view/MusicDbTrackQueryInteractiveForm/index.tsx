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
const { bindTrigger, bindMenu } = require("material-ui-popup-state/hooks");

export interface MusicDbTrackQueryInteractiveFormProps {
  className?: string;
  input: MusicDbTrackQueryForm;
  onChange: (i: MusicDbTrackQueryForm) => void;
}

type Field = keyof MusicDbTrackQueryForm

const useStyles = makeStyles(() => ({
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
}));

export const MusicDbTrackQueryInteractiveForm = (p: MusicDbTrackQueryInteractiveFormProps) => {

  const styles = useStyles();
  const { values, isFieldActive, onFieldChange, setFieldInactive } = useMusicDbTrackQueryFormLogic(p.input)
  const availableTags = ['trance', 'techno', 'house', 'acid']

  return (
    <div className={p.className}>

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