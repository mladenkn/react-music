import { useState } from "react";
import { TrackData, LoadedTracksResponse } from "../../dataModels";
import { fetchTracks as fetchTracks_, fetchTracksFromYT as fetchTracksFromYT_, saveTracks, fetchRelatedTracks } from "../apiClient";
import { replaceMatches } from "../../utils";
import { YoutubeTrackQueryForm, YoutubeTrackQueryFormData } from "../view/YoutubeTrackQueryForm";
import { TrackDataForm } from "../view/TrackQueryInteractiveForm/rootLogic";
    
const tracksPageSize = 20

export const useHomeLogic = () => {

	const [state, setState] = useState({
        tracks: {
            data: [] as TrackData[],
            totalCount: 0
        },
        trackRecommendations: [] as TrackData[],
        fetchedTracksAlready: false,
        playingTrackId: '',
        playingTrackPlaysImmediately: false,
        userPermissions: {
            canEditTrackData: false,
            canFetchTrackRecommendations: false
        }
    });

    const allTracks = state.tracks.data.concat(state.trackRecommendations)

    console.log(state)

    const fetchTracks = async (q: TrackDataForm) => 
    {
        fetchTracks_({ ...q, skip: 0, take: tracksPageSize })
            .then(response => {
                if (response.data && response.data.length !== 0)
                    setState({ ...state, tracks: response, userPermissions: response.permissions });
            })
    }
    
    const fetchMoreTracks = (q: TrackDataForm) => 
        fetchTracks_({...q, skip: state.tracks.data.length, take: tracksPageSize})
            .then(({data, totalCount, permissions}) => 
                {
                    if(data && data.length !== 0){
                        const tracks_ = { 
                            data: state.tracks.data.concat(data),
                            totalCount: totalCount
                        }
                        setState({...state, tracks: tracks_, userPermissions: permissions})
                    }
                }                
            )

    const fetchTracksFromYT = (f: YoutubeTrackQueryFormData) => 
        fetchTracksFromYT_({...f, maxResults: 50})
            .then(response => 
                {
                    const tracks = { data: response.tracks, totalCount: response.tracks.length}
                    if(response.tracks && response.tracks.length !== 0)
                        setState({...state, tracks, userPermissions: response.permissions})
                }                
            )

  const playTrack = (id: string) => setState({ ...state, playingTrackPlaysImmediately: true, playingTrackId: id })
 
  const fetchRecommendationsOf = (trackId: string) => {
    fetchRelatedTracks(trackId)
        .then(r => setState({ ...state, trackRecommendations: r.tracks, userPermissions: r.permissions }))
  }

  const onSaveTrack = async (track: TrackData) => {
    const permissions = await saveTracks([track])
    const tracksData =
        replaceMatches(state.tracks.data, t => t.ytId === track.ytId, track).allItems
    const trackRecommendations =
        replaceMatches(state.trackRecommendations, t => t.ytId === track.ytId, track).allItems
    const tracks_ = { data: tracksData, totalCount: state.tracks.totalCount }
    setState({...state, tracks: tracks_, trackRecommendations, userPermissions: permissions})
  }

  return { 
      ...state,
      allTracks,
      onSaveTrack, 
      fetchTracks,
      fetchTracksFromYT,
      fetchRecommendationsOf,
      playTrack,
      fetchMoreTracks,
    }
} 