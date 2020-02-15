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

export const TrackListUI = (p: TrackListProps) => {
  const classes = useTrackListStyles()
  const onScroll = createOnScrollListener(({isOnBottom}) => {
    if(isOnBottom)
      p.onScrollToBottom()
  })
  return (
    <div className={p.className}>
      {p.tracksTotalCount ?
        <Typography className={classes.trackCount}>
          Showing {p.tracks.length} of {p.tracksTotalCount}
        </Typography> :
        ''
      }
      <List className={clsx(classes.list, p.listClassName)} onScroll={onScroll} disablePadding>
        {p.tracks.map(t => (
          <ListItem disableGutters key={t.youtubeVideoId}>
            <TrackUI 
              fetchRecommendationsOf={p.fetchRecommendationsOf} 
              track={t} 
              onPlay={() => p.onPlayTrack(t.youtubeVideoId)} 
              onSave={editedProps => p.onSaveTrack({...editedProps, id: t.youtubeVideoId})} 
              onClick={() => p.onItemClick(t.youtubeVideoId)}
              isFocused={t.isSelected}
              classes={p.trackClasses}
            />
          </ListItem>
        ))}
      </List>
    </div>
  )
}