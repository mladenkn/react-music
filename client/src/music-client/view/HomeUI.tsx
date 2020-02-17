import { makeStyles } from "@material-ui/core"
import React from 'react'
import { TrackListUI } from "./Tracklist";
import clsx from 'clsx'
import { ems, percent } from "../../utils/css";
import { TrackQueryFormUi } from "./TrackQueryForm";
import { useHomeLogic } from "../logic/home";
import { TrackPlayerUI } from "./TrackPlayer";
import { TrackQueryFormDataSource } from "../shared/trackQueryForm";
 
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
    width: ems(24),
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

  console.log(logic)

  const onScrollToBottom = () => {
    if(logic.queryForm.dataSource === TrackQueryFormDataSource.MusicDb)
      logic.fetchTracksNextPage()
  }

  return (
      <div className={clsx(classes.root, p.className)}>
        <TrackQueryFormUi
          values={logic.queryForm}
          className={classes.form} 
          onChange={logic.setQueryForm} 
        />
        {logic.tracks && (
          <div className={classes.results}>  
            <TrackListUI
              className={classes.trackListRoot} 
              listClassName={classes.trackListList}
              tracks={logic.tracks} 
              tracksTotalCount={logic.tracksTotalCount}
              onPlayTrack={logic.setCurrentTrack}
              onSaveTrack={() => {}}
              onItemClick={logic.onTrackClick}
              fetchRecommendationsOf={() => {}}
              onScrollToBottom={onScrollToBottom}
              saveTrack={logic.saveTrack}
            />
          </div> 
        )}
        <TrackPlayerUI 
          width={380}
          height={215}
          playImmediately={true} 
          videoId={logic.currentTrackYoutubeId || ''} 
        />
      </div>
  )
}