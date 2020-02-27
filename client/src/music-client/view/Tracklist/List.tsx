import { makeStyles, List, ListItem, Typography } from "@material-ui/core"
import React from 'react'
import { TrackUI, TrackUIClasses } from "./TrackUI"
import { ems } from "../../../utils/css";
import { createOnScrollListener } from "../../../utils/view";
import clsx from "clsx";
import { TrackViewModel, SaveTrackModel } from "../../shared/track";

interface TrackListProps {
  className?: string
  trackClasses?: TrackUIClasses
  listClassName: string
  tracks: TrackViewModel[]
  tracksTotalCount?: number
  onPlayTrack: (trackId: number) => void
  onItemClick: (trackId: number) => void
  fetchRecommendationsOf: (trackId: number) => void
  onScrollToBottom: () => void
  saveTrack(t: SaveTrackModel): Promise<void>
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

export const TrackListUI = (props: TrackListProps) => {
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
            <TrackUI 
              fetchRecommendationsOf={props.fetchRecommendationsOf} 
              track={t} 
              onPlay={() => props.onPlayTrack(t.id)}
              onClick={() => props.onItemClick(t.id)}
              isFocused={t.isSelected}
              classes={props.trackClasses}
              saveTrack={editedProps => props.saveTrack({...editedProps, trackId: t.id})}
            />
          </ListItem>
        ))}
      </List>
    </div>
  )
}