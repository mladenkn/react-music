import React from 'react'
import { useTrackQueryFormLogic } from "../logic/trackQueryForm"
import { TrackQueryForm } from "../shared"
import { Select, MenuItem, FormControl, Typography, TextField, makeStyles, createStyles } from "@material-ui/core"

interface TrackQueryFormUiProps {
    className?: string
    form: TrackQueryForm
    onChange: (f: TrackQueryForm) => void
}

const styles = makeStyles(() => createStyles({
    
}))

export const TrackQueryFormUi = (props: TrackQueryFormUiProps) => {
    const logic = useTrackQueryFormLogic()
    return (
        <div className={props.className}>
            <Select 
                label='Data source'
                value='MusicDb'
            >
                <MenuItem value='MusicDb'>Music DB</MenuItem>
                <MenuItem value='YouTube'>YouTube</MenuItem>
            </Select>
            <TextField label='Title contains'></TextField>
            <Select 
                label='YouTube channel'
                value='2trancental'
            >
                <MenuItem value='2trancental'>2trancental</MenuItem>
                <MenuItem value='watchingyourtv'>WatchingYourTv</MenuItem>
                <MenuItem value='marcou99'>marcou99</MenuItem>
            </Select>
        </div>
    )
}