import { makeStyles, IconButton } from "@material-ui/core"
import React from 'react'
import { TrackListUI } from "./Tracklist";
import clsx from 'clsx'
import { ems, percent } from "../../utils/css";
import { HomeSectionOptionsUI } from "./HomeSectionOptions";
import { useHomeLogic } from "../logic/home";
import { TrackPlayerUI } from "./TrackPlayer";
import { TrackQueryFormDataSource } from "../shared/homeSectionOptions";
import ArrorLeftIcon from "@material-ui/icons/ArrowLeft"
 
const useHomeStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    maxWidth: ems(110),
    justifyContent: 'space-between',
  },
  queryTabs: {
  },
  resultsTabs: {
    marginBottom: ems(1)    
  },
  tracklistAndHidButton: {
    display: 'flex',
    alignItems: 'center',
  },
  trackListRoot: { 
    width: ems(40),
    fontSize: ems(1),
  },
  hideTracklistButton: {
    height: ems(2),
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
  const { options } = logic

  console.log(logic)

  const onScrollToBottom = () => {
    if(options.tracksQueryForm.dataSource === TrackQueryFormDataSource.MusicDb && !options.tracksQueryForm.musicDbParams!.randomize)
      logic.fetchTracksNextPage()
  }

  return (
      <div className={clsx(classes.root, p.className)}>
        <HomeSectionOptionsUI
          values={options}
          className={classes.form} 
          onChange={logic.setQueryForm} 
          onSearch={logic.fetchTracks}
        />
        {logic.tracks && (
          <div className={classes.tracklistAndHidButton}>
            <TrackListUI
              className={classes.trackListRoot} 
              listClassName={classes.trackListList}
              tracks={logic.tracks} 
              tracksTotalCount={logic.tracksTotalCount}
              onPlayTrack={logic.setCurrentTrack}
              onItemClick={logic.onTrackClick}
              fetchRecommendationsOf={() => {}}
              onScrollToBottom={onScrollToBottom}
              saveTrack={logic.saveTrack}
            />
            <IconButton className={classes.hideTracklistButton}>              
              <ArrorLeftIcon/>
            </IconButton>
          </div> 
        )}
        <TrackPlayerUI 
          width={380}
          height={215}
          playImmediately
          videoId={logic.currentTrackYoutubeId || ''} 
          onTrackEnd={logic.onCurrentTrackFinish}
        />
      </div>
  )
}