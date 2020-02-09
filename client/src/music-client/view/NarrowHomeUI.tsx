import React, { useState } from 'react';
import { HomeProps } from "./HomeUI";
import { makeStyles } from '@material-ui/styles';
import { TrackListUI } from './Tracklist';
import { ems, percent } from '../../utils/css';
import { Button, Dialog, MenuItem, Select } from '@material-ui/core';
import { TrackPlayerUI } from './TrackPlayer';
import { YoutubeTrackQueryForm } from './YoutubeTrackQueryForm';
import { QueryTypeSelection } from '../logic/homeView';
import { MusicDbTrackQueryInteractiveForm } from './MusicDbTrackQueryInteractiveForm';

const useNarrowHomeUIStyles = makeStyles({
  root: {
    fontSize: ems(0.65),
    padding: ems(0.5),
    display: 'flex',
    alignItems: 'center',
    flexDirection: 'column',
  },
  searchButtonContainer: {    
    height: ems(3.5),
    marginTop: ems(0.4),
    display: 'flex',
    justifyItems: 'start',
    width: percent(100),
  },  
  userKeyInput: {
    width: percent(37),
    height: percent(100),
  },  
  trackPlayer: {
    marginTop: ems(1),
  },
  trackListRoot: {
    marginTop: ems(1),
  },
  trackListList: {
    height: '80vh',
  },
  trackNormalFontSize: {
    fontSize: ems(1.2)
  }, 
  trackPaper: {
    padding: ems(0.2, 0.5, 0)
  },

  queryModalContent: {
    fontSize: ems(0.87),
    padding: ems(0.5),
    display: 'flex',
    flexDirection: 'column',
  },
  queryModalQueryTypeSelector: {
    fontSize: ems(1),    
  },
  queryModalSubmitButton: {
    marginTop: ems(0.8)
  },
}, {name: 'NarrowHomeUI'})
 
export const NarrowHomeUI = (p: HomeProps) => {
  const classes = useNarrowHomeUIStyles()
  const [queryModalOpen, setQueryModalOpen] = useState(false)
  return (
    <div className={classes.root}>
      {/* <Dialog onClose={() => setQueryModalOpen(false)} open={queryModalOpen}>
        <div className={classes.queryModalContent}>        
          <Select 
            className={classes.queryModalQueryTypeSelector}
            value={p.logic.querySelection} 
            onChange={e => p.logic.setQuerySelection(e.target.value as QueryTypeSelection)}
          >
            <MenuItem value={QueryTypeSelection.TrackData}>Normal query</MenuItem>
            <MenuItem value={QueryTypeSelection.YTTrackData}>YT query</MenuItem>
          </Select>
          {p.logic.querySelection === QueryTypeSelection.TrackData &&
            <TrackQueryInteractiveForm input={p.logic.trackDataFormState} onChange={p.logic.trackDataFormChange} />
          } 
          {p.logic.querySelection === QueryTypeSelection.YTTrackData && 
            <YoutubeTrackQueryForm input={p.logic.youtubeTrackForm} onChange={p.logic.setyoutubeTrackForm} />
          }
          <Button 
            className={classes.queryModalSubmitButton} 
            variant="contained" 
            color="primary" 
            onClick={() => {
              p.logic.exeQuery()
              setQueryModalOpen(false)
            }}
          >
            Search
          </Button>
        </div>
      </Dialog>
      <div className={classes.searchButtonContainer}>
        <UserKeyInput className={classes.userKeyInput} />
        <Button           
          onClick={() => setQueryModalOpen(true)} 
          variant='contained' 
          color='primary'
        >
            Search
        </Button>
      </div>
      <TrackPlayerUI 
        className={classes.trackPlayer}
        width='100%' 
        height='20%' 
        videoId={p.logic.playingTrackId} 
        playImmediately={!!p.logic.playingTrackId}
      />
      <TrackListUI
        className={classes.trackListRoot}
        listClassName={classes.trackListList}
        trackClasses={{
          normalFontSize: classes.trackNormalFontSize,
          paper: classes.trackPaper
        }}
        tracks={p.logic.displayedTracks}
        tracksTotalCount={p.logic.tracksTotalCount}
        onPlayTrack={p.logic.playTrack}
        onSaveTrack={p.logic.onSaveTrack}
        onItemClick={p.logic.onItemClick}
        fetchRecommendationsOf={p.logic.fetchRecommendationsOf} 
        selectedItemId={p.logic.selectedItemId}
        onScrollToBottom={p.logic.onTracksScrollToBottom}
      /> */}
    </div>
  )
}