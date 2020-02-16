import { makeStyles, List, ListItem, Typography } from "@material-ui/core"
import React from 'react'
import { TrackUI, TrackUIClasses } from "./TrackUI"
import { ems } from "../../../utils/css";
import { createOnScrollListener } from "../../../utils/view";
import clsx from "clsx";
import { TrackViewModel, TrackEditableProps } from "../../shared/track";

interface TrackListProps {
  className?: string
  trackClasses?: TrackUIClasses
  listClassName: string
  tracks: TrackViewModel[]
  tracksTotalCount?: number
  onPlayTrack: (trackId: string) => void
  onSaveTrack: (t: TrackEditableProps & {id: string}) => void
  onItemClick: (trackId: string) => void
  fetchRecommendationsOf: (trackId: string) => void
  onScrollToBottom: () => void
}

const useTrackListStyles = makeStyles(() => ({
  list: {
    overflowY: 'scroll',
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
          <ListItem disableGutters key={t.youtubeVideoId}>
            <TrackUI 
              fetchRecommendationsOf={props.fetchRecommendationsOf} 
              track={t} 
              onPlay={() => props.onPlayTrack(t.youtubeVideoId)} 
              onSave={editedProps => props.onSaveTrack({...editedProps, id: t.youtubeVideoId})} 
              onClick={() => props.onItemClick(t.youtubeVideoId)}
              isFocused={t.isSelected}
              classes={props.trackClasses}
            />
          </ListItem>
        ))}
      </List>
    </div>
  )
}