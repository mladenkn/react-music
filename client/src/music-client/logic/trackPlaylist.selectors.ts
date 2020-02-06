import { History } from "../../utils/es/history";
import {
  fetchedTracksFromMusicDb,
  fetchedTracksFromYouTube,
  fetchTracksFailed,
  initiatedTracksFetch,
  fetchedTracksNextPage
} from "./trackPlaylist.events";
import { AsyncOperationStatus } from "../../utils/types";

export const tryExtractMusicDbPlaylistState = (history: History) => {
  const lastRequest = history.latestWhereType(initiatedTracksFetch)!;

  if (lastRequest.payload.data.dataSource !== "MusicDb") return undefined;

  const lastRequestId = lastRequest.payload.id;
  const fetchedEvent = history
    .whereType(fetchedTracksFromMusicDb)
    .find(e => e.payload.requestId === lastRequestId);
  if (fetchedEvent) {
    return {
      list: fetchedEvent.payload.data,
      status: "PROCESSED" as AsyncOperationStatus
    };
  }

  return (
    tryExtractErrorStateForRequest(history, lastRequestId) ||
    processingRequestState
  );
};

export const tryExtractYoutubePlaylistState = (history: History) => {
  const lastRequest = history.latestWhereType(initiatedTracksFetch)!;

  if (lastRequest.payload.data.dataSource !== "YouTube") return undefined;

  const lastRequestId = lastRequest.payload.id;
  const fetchedEvent = history
    .whereType(fetchedTracksFromYouTube)
    .find(e => e.payload.requestId === lastRequestId);
  if (fetchedEvent) {
    return {
      list: fetchedEvent.payload.data,
      status: "PROCESSED" as AsyncOperationStatus
    };
  }

  return (
    tryExtractErrorStateForRequest(history, lastRequestId) ||
    processingRequestState
  );
};

const tryExtractErrorStateForRequest = (
  history: History,
  requestId: number
) => {
  const failedEvent = history
    .whereType(fetchTracksFailed)
    .find(e => e.payload.requestId === requestId);
  return (
    failedEvent && {
      list: undefined,
      status: "ERROR" as AsyncOperationStatus
    }
  );
};

export const getLastMusicDbFetchId = (history: History) => {
  const event = history.latestWhereType(fetchedTracksNextPage)!;
  return event && event.payload.id;
};

export const getNumberOfTracksFetched = (history: History) => {
  const initialLoad = history.latestWhereType(fetchedTracksFromMusicDb);
  if (!initialLoad) return 0;
  const nextPages = history
    .whereType(fetchedTracksNextPage)
    .filter(e => e.payload.initialRequestId === initialLoad.payload.requestId);
  return (
    initialLoad.payload.data.data.length +
    nextPages.reduce((total, event) => total + event.payload.data.data.length, 0)
  );
};

const processingRequestState = {
  list: undefined,
  status: "ERROR" as AsyncOperationStatus
};
