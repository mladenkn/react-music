import { makeStyles, Tabs, Tab, Typography } from "@material-ui/core"
import React from 'react'
import { TrackListUI } from "./Tracklist";
import clsx from 'clsx'
import { TrackPlayerUI } from "./TrackPlayer";
import { ems, percent } from "../../utils/css";
import { YoutubeTrackQueryForm } from "./YoutubeTrackQueryForm";
import { useTracklistLogic } from "../logic/tracklist.decorated";
import { TrackQueryFormUi } from "./TrackQueryForm";
import { useHomeLogic } from "../logic/home";
 
const useHomeStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    justifyContent: 'space-between',
  },
  queryTabs: {
  },
  results: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  resultsTabs: {
    marginBottom: ems(1)    
  },
  trackListRoot: { 
    width: ems(40),
    fontSize: ems(1),
    padding: ems(0, 1),
  },
  trackListList: {
    height: '88vh',
    padding: ems(0, 0.5)
  },
  form: {
    maxWidth: ems(24),
    fontSize: ems(1.1),
    padding: ems(1.5, 0.5, 0),
  },
  ytForm: {
    marginTop: ems(2),
  },
  tab: {
    fontSize: ems(1)
  },
  exeQueryAction: {
    float: 'right',
    clear: 'both',
    marginTop: ems(1),
    fontSize: ems(0.9),
  },
  setAdminKeyButton: {
    height: '3vh',
    width: percent(30),
    position: 'absolute',
    bottom: 0,
  },
}), {name: 'Home'})
 
export interface HomeProps {
  className?: string
}

export const HomeUI = (p: HomeProps) => {
  const classes = useHomeStyles()
  const logic = useHomeLogic()

  const TrackList = () => {
    switch(logic.tracksStatus){
      case 'ERROR':
        return <Typography>Error</Typography>
      case 'NOT_INITIALIZED':
        return <Typography>Loading</Typography>
      case 'PROCESSING':
        return <Typography>Loading</Typography>
      case 'PROCESSED':
        const tracks = logic.queryForm.dataSource === 'MusicDb' ?
          logic.fromMusicDb!.list!.data :
          logic.fromYouTube!.list!    

        const tracksTotalCount = logic.queryForm.dataSource === 'MusicDb' ? 
          logic.fromMusicDb!.list!.totalCount : 
          undefined
      
        return (
          <div className={classes.results}>  
            <TrackListUI                     
              className={classes.trackListRoot} 
              listClassName={classes.trackListList}
              tracks={tracks} 
              tracksTotalCount={tracksTotalCount}
              onPlayTrack={() => {}}
              onSaveTrack={() => {}}
              onItemClick={logic.onTrackClick}
              fetchRecommendationsOf={() => {}} 
              selectedItemId={logic.selectedTrackId}
              onScrollToBottom={logic.fetchTracksNextPage}
            />
          </div> 
        )
    }
  }

  return (
      <div className={clsx(classes.root, p.className)}>
        <TrackQueryFormUi
          form={logic.queryForm}
          className={classes.form} 
          onChange={() => {}} 
        />
        <TrackList />
        {/* <TrackPlayerUI 
          width={380}
          height={215}
          playImmediately={p.logic.playingTrackPlaysImmediately} 
          videoId={p.logic.playingTrackId} 
        /> */}
      </div>
  )
}