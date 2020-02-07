import { makeStyles, Typography, Grid, InputLabel } from "@material-ui/core"
import React from 'react'
import clsx from 'clsx'
import { Formik } from "formik";
import { ems, percent } from "../../utils/css";

interface TrackData {
  title: string
  id: string
  year?: number
  genres: string[]
  tags: string[]  
}

interface TrackFormUIProps {
  className?: string
  track: TrackData
}

export const useTrackFormLogic = () => {
  const onChange = (i: TrackData) => {
    console.log(i)
  }
  return { onChange }
}

const useTrackFormUIStyles = makeStyles(() => ({
	root: {
    display: 'flex',
    flexDirection: 'column',
  },
  fontSize: {
    fontSize: ems(1),
  },
  readonlyProp: { 
    display: 'flex',
    alignItems: 'center',
    marginBottom: ems(1),
  },
  readonlyProp_value: {
    marginLeft: percent(3)
  },
  chipListEditor: {
    padding: ems(0.2, 0, 0.3),
    widht: percent(100),
  },
  yearProp: {
    display: 'flex',
    alignItems: 'center',
    marginBottom: ems(1),    
  },
  yearValue: {
    marginLeft: percent(3),
    width: ems(4),
  },
  chip: {
    fontSize: ems(0.8),
  },
}), {name: 'TrackFormUI'})

export const TrackFormUI = (p: TrackFormUIProps) => {
  const classes = useTrackFormUIStyles()
  const logic = useTrackFormLogic()
	return (
    <Formik initialValues={p.track} validate={logic.onChange} onSubmit= {() => {}}>
      {({values, handleChange}) => (
        <div className={clsx(classes.root, p.className)}>
            <div className={classes.readonlyProp}>
              <InputLabel className={classes.fontSize}>Id:</InputLabel>
              <Typography className={clsx(classes.readonlyProp_value, classes.fontSize)}>{values.id}</Typography>
            </div>
            <div className={classes.readonlyProp}>
              <InputLabel className={classes.fontSize}>Title:</InputLabel>
              <Typography className={clsx(classes.readonlyProp_value, classes.fontSize)}>{values.title}</Typography>
            </div>
            <div className={classes.yearProp}>
              <InputLabel className={classes.fontSize}>Year:</InputLabel>
              {/* <TextField className={classes.yearValue} type='number' name='year' /> */}
            </div>
            {/* <ChipListEditor 
              fullWidth
              className={classes.chipListEditor}
              classes={{
                label: classes.fontSize,
                chip: classes.chip,
              }}
              label='Genres'
              initialValue={values.genres} 
              onChange={value => handleChange({ target: {name: 'genres', value } } )}
            />
            <ChipListEditor 
              fullWidth
              className={classes.chipListEditor}
              classes={{
                label: classes.fontSize,
                chip: classes.chip,
              }}
              label='Tags'
              initialValue={values.tags} 
              onChange={value => handleChange({ target: {name: 'tags', value } } )}
            /> */}
        </div>
      )}
    </Formik>
	)
}
