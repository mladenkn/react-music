import { TrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { ArrayWithTotalCount, AsyncOperationStatus } from "../../utils/types";
import { useHistory, History } from "../../utils/es/history";
import {
  updatedQueryTrackForm,
  initiatedTracksFetch,
  fetchTracksFailed,
  fetchedTracksFromMusicDb,
  fetchedTracksFromYouTube
} from "./trackPlaylist.events";
import { useRequestIdGenerator } from "./requestIdGenerator";

export interface TrackPlaylist {
  queryForm: TrackQueryForm;
  fromMusicDb?: {
    list?: ArrayWithTotalCount<Track>;
    status: AsyncOperationStatus;
  };
  fromYouTube?: {
    list?: Track[];
    status: AsyncOperationStatus;
  };
  setQueryForm(form: TrackQueryForm): void;
  saveTrack(t: Track): Promise<void>;
}

const pageSize = 20;

export const useTrackPlaylist = (): TrackPlaylist => {
  const history = useHistory();

  const updatedQueryTrackFormEvents = history.whereType(updatedQueryTrackForm);
  const nextRequestId = useRequestIdGenerator();

  useEffect(() => {
    const lastUpdatedQueryTrackForm =
      updatedQueryTrackFormEvents[updatedQueryTrackFormEvents.length - 1];
    const requestId = nextRequestId();
    history.save(initiatedTracksFetch({ data: lastUpdatedQueryTrackForm.payload, id: requestId }));

    if (lastUpdatedQueryTrackForm.payload.dataSource === "MusicDb")
      tracksApi
        .fetchFromMusicDb({
          ...lastUpdatedQueryTrackForm.payload.fields!,
          take: pageSize,
          skip: 0
        })
        .then(r => history.save(fetchedTracksFromMusicDb({ data: r.data, requestId })))
        .catch(() => history.save(fetchTracksFailed(requestId)));
    else
      tracksApi
        .fetchFromYT(lastUpdatedQueryTrackForm.payload.searchQuery!)
        .then(r => history.save(fetchedTracksFromYouTube({ data: r.data, requestId })))
        .catch(() => history.save(fetchTracksFailed(requestId)));

  }, [updatedQueryTrackFormEvents.length]);

  let fromMusicDb: TrackPlaylist['fromMusicDb'];
  let fromYouTube: TrackPlaylist['fromYouTube'];

  const queryForm = history.whereTypeSingle(updatedQueryTrackForm)!.payload;

  if(queryForm.dataSource === 'MusicDb')
    fromMusicDb = extractMusicDbPlaylistInfo(history)
  else 
    fromYouTube = extractYoutubePlaylistInfo(history)

  const setQueryForm = (form: TrackQueryForm) => history.save(updatedQueryTrackForm(form));

  const saveTrack = (t: Track) => tracksApi.save(t);

  return { queryForm, fromMusicDb, fromYouTube, setQueryForm, saveTrack }
};

const extractMusicDbPlaylistInfo = (history: History) => {
  const lastRequest = history.whereTypeSingle(initiatedTracksFetch)!

  if(lastRequest.payload.data.dataSource !== 'MusicDb')
    return undefined

  const lastRequestId = lastRequest.payload.id
  const fetchedEvent = history.whereType(fetchedTracksFromMusicDb).find(e => e.payload.requestId === lastRequestId)
  if(fetchedEvent){
    return {
      list: fetchedEvent.payload.data,
      status: 'PROCESSED' as AsyncOperationStatus
    }
  }
  
  const failedEvent = history.whereType(fetchTracksFailed).find(e => e.payload.requestId === lastRequestId)
  if(failedEvent)
    return {
      list: undefined,
      status: 'ERROR' as AsyncOperationStatus
    }

  return {
    list: undefined,
    status: 'PROCESSING' as AsyncOperationStatus
  }
}

const extractYoutubePlaylistInfo = (history: History) => {
  const lastRequest = history.whereTypeSingle(initiatedTracksFetch)!

  if(lastRequest.payload.data.dataSource !== 'YouTube')
    return undefined

    const lastRequestId = lastRequest.payload.id
    const fetchedEvent = history.whereType(fetchedTracksFromYouTube).find(e => e.payload.requestId === lastRequestId)
    if(fetchedEvent){
      return {
        list: fetchedEvent.payload.data,
        status: 'PROCESSED' as AsyncOperationStatus
      }
    }
    
    const failedEvent = history.whereType(fetchTracksFailed).find(e => e.payload.requestId === lastRequestId)
    if(failedEvent)
      return {
        list: undefined,
        status: 'ERROR' as AsyncOperationStatus
      }
  
    return {
      list: undefined,
      status: 'PROCESSING' as AsyncOperationStatus
    }
}