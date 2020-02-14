import { History } from "../../utils/es/history"
import {
  fetchedTracksFromMusicDb,
  fetchedTracksFromYouTube,
  fetchTracksFailed,
  initiatedTracksFetch,
  fetchedTracksNextPage,
  selectedTrack,
  updatedQueryTrackForm
} from "./tracklist.events"
import { AsyncOperationStatus } from "../../utils/types"

export const didRequest = (history: History) => {
  return !!history.latestWhereType(initiatedTracksFetch)
}

export const tryExtractMusicDbTracklistState = (history: History) => {
  const lastRequest = history.latestWhereType(initiatedTracksFetch)!

  if (lastRequest.payload.data.searchQuery) return undefined

  const lastRequestId = lastRequest.payload.id
  const fetchedEvent = history
    .whereType(fetchedTracksFromMusicDb)
    .find(e => e.payload.requestId === lastRequestId)
  if (fetchedEvent) {
    return {
      list: fetchedEvent.payload.data,
      status: "PROCESSED" as AsyncOperationStatus
    }
  }

  return (
    tryExtractErrorStateForRequest(history, lastRequestId) ||
    processingRequestState
  )
}

export const tryExtractYoutubeTracklistState = (history: History) => {
  const lastRequest = history.latestWhereType(initiatedTracksFetch)!

  if (lastRequest.payload.data.fields) return undefined

  const lastRequestId = lastRequest.payload.id
  const fetchedEvent = history
    .whereType(fetchedTracksFromYouTube)
    .find(e => e.payload.requestId === lastRequestId)
  if (fetchedEvent) {
    return {
      list: fetchedEvent.payload.data,
      status: "PROCESSED" as AsyncOperationStatus
    }
  }

  return (
    tryExtractErrorStateForRequest(history, lastRequestId) ||
    processingRequestState
  )
}

const tryExtractErrorStateForRequest = (
  history: History,
  requestId: number
) => {
  const failedEvent = history
    .whereType(fetchTracksFailed)
    .find(e => e.payload.requestId === requestId)
  return (
    failedEvent ?
      {
        list: undefined,
        status: "ERROR" as AsyncOperationStatus
      } : 
      undefined
  )
}

export const getLastMusicDbFetchId = (history: History) => {
  const event = history.latestWhereType(fetchedTracksNextPage)!
  return event && event.payload.id
}

export const getNumberOfTracksFetched = (history: History) => {
  const initialLoad = history.latestWhereType(fetchedTracksFromMusicDb)
  if (!initialLoad) return 0
  const nextPages = history
    .whereType(fetchedTracksNextPage)
    .filter(e => e.payload.initialRequestId === initialLoad.payload.requestId)
  return (
    initialLoad.payload.data.data.length +
    nextPages.reduce(
      (total, event) => total + event.payload.data.data.length,
      0
    )
  )
}

export const getSelectedTrackYoutubeId = (history: History) => {
  const lastEvent = history.latestWhereType(selectedTrack)
  if(!lastEvent)
    return undefined
  return lastEvent.payload.trackYoutubeId;
}

export const getLatestQueryForm = (history: History) => {
  const event = history.latestWhereType(updatedQueryTrackForm)
  return event && event.payload
}

const processingRequestState = {
  list: undefined,
  status: "ERROR" as AsyncOperationStatus
}
