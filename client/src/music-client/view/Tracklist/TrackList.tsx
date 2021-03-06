import { makeStyles, List, ListItem, Typography } from "@material-ui/core"
import React from 'react'
import { Track, TrackClasses } from "./Track"
import { ems } from "../../../utils/css";
import { createOnScrollListener } from "../../../utils/view";
import clsx from "clsx";
import { TrackViewModel, SaveTrackModel } from "../../shared/track";

interface TrackListProps {
  className?: string
  trackClasses?: TrackClasses
  listClassName: string
  tracks: TrackViewModel[]
  tracksTotalCount?: number
  onPlayTrack: (trackId: number) => void
  onItemClick: (trackId: number) => void
  fetchRecommendationsOf: (trackId: number) => void
  onScrollToBottom: () => void
  saveTrack(t: SaveTrackModel): Promise<void>
  declareANonTrack(videoId: string): void
}

const useTrackListStyles = makeStyles(() => ({
  list: {
  },
  trackCount: {
    display: 'flex',
    justifyContent: 'flex-end',
    fontSize: ems(1.25),
  },
}), {name: 'TrackList'})

export const TrackList = (props: TrackListProps) => {
  const classes = useTrackListStyles()  
  
  const onScroll = createOnScrollListener(({isOnBottom, scrollTop}) => {
    if(isOnBottom)
      props.onScrollToBottom()
  })

  return (
    <div className={props.className}>
      {props.tracksTotalCount ?
        <Typography className={classes.trackCount}>
          Showing {props.tracks.length} of {props.tracksTotalCount}
        </Typography> :
        ''
      }
      <List className={clsx(classes.list, props.listClassName)} onScroll={onScroll} disablePadding>
        {props.tracks.map(t => (
          <ListItem disableGutters key={t.id}>
            <Track 
              fetchRecommendationsOf={() => props.fetchRecommendationsOf(t.id)}
              track={t} 
              onPlay={() => props.onPlayTrack(t.id)}
              onClick={() => props.onItemClick(t.id)}
              isFocused={t.isSelected}
              classes={props.trackClasses}
              saveTrack={editedProps => props.saveTrack({...editedProps, trackId: t.id})}
              declareANonTrack={() => props.declareANonTrack(t.youTubeVideoId)}
            />
          </ListItem>
        ))}
      </List>
    </div>
  )
}