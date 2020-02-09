import React from 'react'
import { TrackQueryForm } from "../shared"
import { Select, MenuItem, FormControl, Typography, TextField, makeStyles, createStyles } from "@material-ui/core"
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm'

interface TrackQueryFormUiProps {
    className?: string
    form: TrackQueryForm
    onChange: (f: TrackQueryForm) => void
}

const styles = makeStyles(() => createStyles({
    
}))

export const TrackQueryFormUi = (props: TrackQueryFormUiProps) => {
    return (
        <div className={props.className}>
            <Select 
                label='Data source'
                value='MusicDb'
            >
                <MenuItem value='MusicDb'>Music DB</MenuItem>
                <MenuItem value='YouTube'>YouTube</MenuItem>
            </Select>
            <MusicDbTrackQueryInteractiveForm input={props.form.fields!} onChange={() => {}} />
        </div>
    )
}