import React from 'react';
import { Track } from '../../shared/track';
import { Chip, Paper } from '@material-ui/core';

type Props = {
  selectedTracks: Track[]
  onRemoveTrack(trackId: number): void
}

export function RelatedTracks(props: Props){
  return (
    <Paper>
      {props.selectedTracks.map(t => (
        <Chip key={t.id} label={t.title} onDelete={() => props.onRemoveTrack(t.id)} />
      ))}
    </Paper>
  )
}