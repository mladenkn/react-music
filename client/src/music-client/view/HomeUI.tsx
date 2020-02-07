import { makeStyles, Tabs, Tab } from "@material-ui/core"
import React from 'react'
import { TrackListUI } from "./Tracklist";
import clsx from 'clsx'
import { TrackPlayerUI } from "./TrackPlayer";
import { ems, percent } from "../../utils/css";
import { YoutubeTrackQueryForm } from "./YoutubeTrackQueryForm";
import { QueryTypeSelection } from "../logic/homeView";
import { UserKeyInput } from "./UserKeyInput";
import { TrackQueryInteractiveForm } from "./TrackQueryInteractiveForm";
 
const useHomeStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    justifyContent: 'space-between',
  },
  querySide: {
    maxWidth: ems(24),
    position: 'relative',
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
  logic: HomeViewLogic
}
 
export const HomeUI = (p: HomeProps) => {
  const classes = useHomeStyles()
  return (
      <div className={clsx(classes.root, p.className)}>
        <div className={classes.querySide}>            
          <Tabs 
            indicatorColor='primary'
            className={classes.queryTabs}
            value={p.logic.querySelection} 
            onChange={(_, v) => p.logic.setQuerySelection(v)}
          >
            <Tab label="Normal query" className={classes.tab} />
            <Tab label="YT query" className={classes.tab} />
          </Tabs>
          {p.logic.querySelection === QueryTypeSelection.TrackData &&
            <TrackQueryInteractiveForm 
              input={p.logic.trackDataFormState}
              className={classes.form} 
              onChange={p.logic.trackDataFormChange} 
            />
          } 
          {p.logic.querySelection === QueryTypeSelection.YTTrackData && 
            <YoutubeTrackQueryForm input={p.logic.youtubeTrackForm} onChange={p.logic.setyoutubeTrackForm} />
          }
          <UserKeyInput className={classes.setAdminKeyButton} />
        </div>
        <div className={classes.results}>  
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
        />
      </div>
  )
}