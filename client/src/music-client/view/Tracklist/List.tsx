import { makeStyles, List, ListItem, Typography } from "@material-ui/core"
import React from 'react'
import { TrackUI, TrackUIClasses } from "./TrackUI"
import { TrackViewData, TrackDataEditableProps } from "../../logic/homeView";
import { ems } from "../../../utils/css";
import { createOnScrollListener } from "../../../utils/components";
import clsx from "clsx";

interface TrackListProps {
  className?: string
  trackClasses?: TrackUIClasses
  listClassName: string
  tracks: TrackViewData[]
  tracksTotalCount: number
  onPlayTrack: (trackId: string) => void
  onSaveTrack: (t: TrackDataEditableProps & {id: string}) => void
  onItemClick: (trackId: string) => void
  fetchRecommendationsOf: (trackId: string) => void
  onScrollToBottom: () => void
  selectedItemId: string | undefined
  displayTrackCount?: boolean
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
  const onScroll = createOnScrollListener({onBottom: p.onScrollToBottom})
  return (
    <div className={p.className}>
      {p.displayTrackCount && <Typography className={classes.trackCount}>Showing {p.tracks.length} of {p.tracksTotalCount}</Typography>      }
      <List className={clsx(classes.list, p.listClassName)} onScroll={onScroll} disablePadding>
        {p.tracks.map(t => (
          <ListItem disableGutters key={t.id}>
            <TrackUI 
              fetchRecommendationsOf={p.fetchRecommendationsOf} 
              track={t} 
              onPlay={() => p.onPlayTrack(t.id)} 
              onSave={editedProps => p.onSaveTrack({...editedProps, id: t.id})} 
              onClick={() => p.onItemClick(t.id)}
              isFocused={p.selectedItemId === t.id}
              classes={p.trackClasses}
            />
          </ListItem>
        ))}
      </List>
    </div>
  )
}