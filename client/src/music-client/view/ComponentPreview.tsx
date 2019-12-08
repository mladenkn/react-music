import { makeStyles, Box, withStyles } from "@material-ui/core"
import React, { ComponentType } from 'react'
import clsx from 'clsx'
import { ElementBrowserUI, ElementBrowserElement } from "../../utils/components/ElementBrowser";
import { AppRootUI } from "./AppRoot";
import { TrackFormUI } from "./TrackFormUI";
import { ems } from "../../utils/css";
import { ChipEditUI } from "../../utils/components/EditableChipList";

export default () => 
  <ElementBrowserUI consoleSwitch>
    <TrackFormUIPreview name="TrackForm" />
    <AppRootUIPreview name="AppRoot" />
    <ElementBrowserElement name="chip edit">
      <ChipEditUI onConfirm={() => {}} />
    </ElementBrowserElement>
  </ElementBrowserUI>
  
const component = (styles: any, c: ComponentType<any>) => withStyles(styles)(c)

const TrackFormUIPreview = component({
  root: {
    padding: ems(2),
    width: ems(22),
    fontSize: ems(0.9),
  }
},
({classes}) => {
  const track = {
    id: 'asdčfjaowefi',
    title: 'The Source Experience - Point Zero (1994)',
    // year: 1993,
    genres: ['trance', 'techno', 'house'],
    tags: ['tag1', 'tag2', 'tag3'],
  }
	return <TrackFormUI className={clsx(classes.root)} track={track} />
})

const AppRootUIPreview = component({
  root: {
    width: '100%',
  },
},
({classes}) => <AppRootUI className={classes.root} />) 