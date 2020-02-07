import React from 'react'
import { makeStyles } from "@material-ui/styles";
import { FormGroup, TextField } from "@material-ui/core";
import { ems } from "../../utils/css";

const useYoutubeTrackQueryFormStyles = makeStyles({
  formElement: {
    padding: ems(0.3, 0)
  },
  fieldLabel: {
    fontSize: ems(1.1)
  },
  fieldValue: {

  }
})

export interface YoutubeTrackQueryFormData {
  searchQuery: string | undefined
  channelTitle: string | undefined
}

export interface Props {
  input: YoutubeTrackQueryFormData
  onChange: (i: YoutubeTrackQueryFormData) => void
}

export const YoutubeTrackQueryForm = (p: Props) => {

  const classes = useYoutubeTrackQueryFormStyles()
  // const { input, onPropChange } = useFormLogic(p.input, p.onChange)
  return (
    <div>
      {/* <FormGroup className={classes.formElement} row>
        <TextField 
          fullWidth
          InputLabelProps={{className: classes.fieldLabel}}
          className={classes.fieldValue} 
          label='Search query'
          value={input.searchQuery}
          onChange={onPropChange('searchQuery')}
        />
      </FormGroup>
      <FormGroup className={classes.formElement} row>
        <TextField 
          InputLabelProps={{className: classes.fieldLabel}}
          className={classes.fieldValue} 
          label='Channel' 
          value={input.channelTitle}
          onChange={onPropChange('channelTitle')}
        />
      </FormGroup> */}
    </div>
  )
}