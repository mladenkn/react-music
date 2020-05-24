import React from "react";
import { ChipListElement } from "./ChipListElement";
import { InlineRangeElement } from "./InlineRangeElement";
import AddIcon from "@material-ui/icons/Add";
import { Menu, MenuItem, Fab, InputLabel } from "@material-ui/core";
import { makeStyles } from "@material-ui/styles";
import { ems, percent } from "../../../utils/css";
import { MusicDbTrackQueryParams } from "../../shared/homeSectionOptions";
import { useMusicDbTrackQueryFormLogic } from "../../logic/musicDbtrackQueryForm";
import { Checkbox } from '@material-ui/core';
import clsx from "clsx";
import { bindTrigger, bindMenu, usePopupState } from "material-ui-popup-state/hooks";
import { TextField } from "./TextField";
import { IdWithName } from "../../../utils/types";
import { SupportedChannelsBuilderElement } from "./SupportedChannelsBuilderElement";
import { Track } from "../../shared/track";
import { RelatedTracks } from "./RelatedTracks";

export interface MusicDbTrackQueryInteractiveFormProps {
  className?: string;
  input: MusicDbTrackQueryParams;
  onChange: (i: MusicDbTrackQueryParams) => void;
  availableTags: string[]
  availableYouTubeChannels: IdWithName[]
  getTracksWithIds(ids: number[]): Track[]
}

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex'
  },
  left: {
    width: percent(83),
    marginRight: percent(5)
  },
  year: {
    width: percent(100)
  },
  titleContainsField: {
    padding: ems(0.5 , 0.6)
  },
  addPropertyButton: {
    marginTop: ems(0.3),
    marginRight: ems(-0.5)
  },
  addPropertyButtonIcon: {
    width: ems(1.2),
    height: ems(1.2)
  },
  randomize: {
    display: 'flex',
    alignItems: 'center',
  },
}), {name: 'MusicDbTrackQueryInteractiveForm'});

type Field = keyof MusicDbTrackQueryParams

const mapFieldValueToName = (field: Field) => {
  switch (field) {
    case "supportedYouTubeChannelsIds":
      return "Supported channels";
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
    availableYouTubeChannels,
    availableTags,
    inactiveFields, 
    isFieldActive, 
    onFieldChange, 
    setFieldActive,
    setFieldInactive,
    removeRelatedTrack
  } = useMusicDbTrackQueryFormLogic({
    values: p.input,
    onChange: p.onChange,
    availableTags: p.availableTags,
    availableYouTubeChannels: p.availableYouTubeChannels
  })
  const popupState = usePopupState({ variant: "popover", popupId: "TrackQueryInteractiveFormPopup" })

  const availableTagsSorted = availableTags.sort()
  
  return (
    <div className={clsx(p.className, styles.root)}>
      <div className={styles.left}>        
        <div className={styles.randomize}>
          <InputLabel>Randomize:</InputLabel>
          <Checkbox
            checked={values.randomize}
            color='primary'
            onChange={e => onFieldChange('randomize')(e.target.checked)}
          />
        </div>

        {isFieldActive('mustHaveEveryTag') && (
            <ChipListElement
              label='Must have all tags'
              availableOptions={availableTagsSorted}
              value={values.mustHaveEveryTag}
              onChange={onFieldChange('mustHaveEveryTag')}
              onRemove={() => setFieldInactive('mustHaveEveryTag')}
            />
          )}

        {isFieldActive('mustHaveAnyTag') && (
          <ChipListElement
            label='Must have some tags'
            availableOptions={availableTagsSorted}
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
          <TextField 
            onRemove={() => setFieldInactive('titleContains')}
            label='Title contains'
            value={values.titleContains}
            onChange={onFieldChange("titleContains")}
          />
        }

        {isFieldActive('supportedYouTubeChannelsIds') &&          
          <SupportedChannelsBuilderElement 
            onRemove={() => setFieldInactive('supportedYouTubeChannelsIds')}
            label='Allowed YouTube channels'
            availableChannels={availableYouTubeChannels}
            value={values.supportedYouTubeChannelsIds}
            onChange={onFieldChange("supportedYouTubeChannelsIds")}
          />
        }

        {isFieldActive('relatedTracks') && (
          <RelatedTracks 
            selectedTracks={p.getTracksWithIds(values.relatedTracks)}
            onRemoveTrack={removeRelatedTrack}
          />
        )}
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