import { makeStyles, FormGroup, TextField } from "@material-ui/core"
import React, { Fragment } from 'react'
import clsx from 'clsx'
import { Formik } from "formik";
import { ems, percent } from "../../utils/css";
import ChipInput from "../../utils/view/ChipInput";

const useTrackQueryFromStyles = makeStyles(() => ({
	root: {
    display: 'flex',
    flexDirection: 'column',
  },
  formElement: {
    padding: ems(0.3, 0)
  },
  titleMatchFormElement: {
    width: percent(100)
  },
  fieldLabel: {
    fontSize: ems(1)
  },
  maxResults: {
    maxWidth: ems(6),
  },
  yearSpan: {
    paddingTop: ems(0.4)
  },
  yearBound: {
    maxWidth: ems(5.5),
  },
  yearMax: {
    marginLeft: ems(1),
  },
  chipListEditor: {
    width: percent(100)
  },
}), {name: 'TrackQueryFrom'})

interface TrackDataForm {
    titleMatch: string | undefined
    channel: string | undefined
    mustContainAllGenres: string[],
    mustContainAllTags: string[],
    yearSpan?: {
        from?: number,
        to?: number
    },
}

interface TrackQueryFromProps {
  className?: string
  input: TrackDataForm
  onChange: (i: TrackDataForm) => void
}

// const useLogic = (p: TrackQueryFromProps) => {  

//   const mapInput = (yearSpanTo: 'incr' | 'decr') => (i: TrackDataForm) => {
//     const num = yearSpanTo === 'incr' ? 1 : -1
//     const mappedYearSpan = i.yearSpan && 
//     { from: i.yearSpan.from, to: i.yearSpan!.to && i.yearSpan!.to + num }
//     return {...i, yearSpan: mappedYearSpan}
//   }

//   const formLogic = useFormLogic(
//     p.input, p.onChange, mapInput('incr'), mapInput('decr')
//   )

//   return formLogic
// }

export const TrackQueryFromUI = (p: TrackQueryFromProps) => {
  const classes = useTrackQueryFromStyles()
  // const { input, onPropChange } = useLogic(p)
 
	return (
    <div className={clsx(classes.root, p.className)}>
      {/* <FormGroup className={classes.formElement} row>
        <ChipInput
          label="Genres"
          className={classes.chipListEditor}
          classes={{
            label: classes.fieldLabel,
          }}
          value={input.mustContainAllGenres}
          onChange={onPropChange('mustContainAllGenres')}        
        />
      </FormGroup>
      <FormGroup className={classes.formElement} row>
        <TextField 
          InputLabelProps={{className: classes.fieldLabel}}
          className={classes.titleMatchFormElement} 
          label='Title' 
          value={input.titleMatch}
          onChange={onPropChange('titleMatch')}
        />
      </FormGroup>
      <FormGroup className={classes.formElement} row>
        <TextField 
          InputLabelProps={{className: classes.fieldLabel}}
          label='Channel' 
          value={input.channel}
          onChange={onPropChange('channel')}
        />
      </FormGroup>
      <FormGroup className={clsx(classes.formElement, classes.yearSpan)} row>            
        <Fragment>
          <TextField
            InputLabelProps={{className: classes.fieldLabel}}
            className={classes.yearBound}
            label="Min year" 
            type="number"
            value={input.yearSpan!.from}
            onChange={onPropChange(['yearSpan', 'from'])}
          />
          <TextField
            InputLabelProps={{className: classes.fieldLabel}}
            className={clsx(classes.yearBound, classes.yearMax)}
            label="Max Year" 
            type="number"
            value={input.yearSpan!.to}
            onChange={onPropChange(['yearSpan', 'to'])}
          />
        </Fragment>
      </FormGroup> */}
    </div>
	)
}