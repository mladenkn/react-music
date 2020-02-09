import { makeStyles, Tabs, Tab } from "@material-ui/core"
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
  return (
      <div className={clsx(classes.root, p.className)}>
        <TrackQueryFormUi
          form={logic.tracklist.queryForm}
          className={classes.form} 
          onChange={() => {}} 
        />
        {/* <div className={classes.results}>  
          <Tabs 
            indicatorColor='primary'
            className={classes.resultsTabs}
            value={p.logic.resultSelection} 
            onChange={(_, v) => p.logic.setResultSelection(v)}
          >
            <Tab label="Query results" className={classes.tab} />
            <Tab label="Recomendations" className={classes.tab} />
          </Tabs>
          <TrackListUI                     
            className={classes.trackListRoot} 
            listClassName={classes.trackListList}
            tracks={p.logic.displayedTracks} 
            tracksTotalCount={p.logic.tracksTotalCount}
            onPlayTrack={p.logic.playTrack}
            onSaveTrack={p.logic.onSaveTrack}
            onItemClick={p.logic.onItemClick}
            fetchRecommendationsOf={p.logic.fetchRecommendationsOf} 
            selectedItemId={p.logic.selectedItemId}
            onScrollToBottom={p.logic.onTracksScrollToBottom}
            displayTrackCount={p.logic.resultSelection !== QueryResultSelection.Recommedations}
          />
        </div> 
        <TrackPlayerUI 
          width={380}
          height={215}
          playImmediately={p.logic.playingTrackPlaysImmediately} 
          videoId={p.logic.playingTrackId} 
        /> */}
      </div>
  )
}