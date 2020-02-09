import { Track, TrackQueryForm, createInitialTrackQueryForm } from "../shared"
import { tracksApi } from "../apiClient"
import { useEffect } from "react"
import { ArrayWithTotalCount, AsyncOperationStatus } from "../../utils/types"
import { useHistory } from "../../utils/es/history"
import {
  updatedQueryTrackForm,
  initiatedTracksFetch,
  fetchTracksFailed,
  fetchedTracksFromMusicDb,
  fetchedTracksFromYouTube,
  fetchedTracksNextPage,
  fetchTracksNextPageFailed,
  selectedTrack
} from "./tracklist.events"
import { useRequestIdGenerator } from "./requestIdGenerator"
import {
  tryExtractMusicDbTracklistState,
  tryExtractYoutubeTracklistState,
  getLastMusicDbFetchId,
  getNumberOfTracksFetched,
  getSelectedTrackYoutubeId,
  getLatestQueryForm,
  didRequest
} from "./tracklist.selectors"

export interface Tracklist {
  queryForm: TrackQueryForm
  fromMusicDb?: {
    list?: ArrayWithTotalCount<Track>
    status: AsyncOperationStatus
  }
  fromYouTube?: {
    list?: Track[]
    status: AsyncOperationStatus
  }
  selectedTrackId?: string
  didRequest: boolean
  fetchTracksNextPage(): void
  setQueryForm(form: TrackQueryForm): void
  saveTrack(t: Track): Promise<void>
  onTrackClick(trackYoutubeId: string): void
}

const fallbacks = {
  queryForm: createInitialTrackQueryForm(),  
}

const pageSize = 20

export const useTracklistLogic = (): Tracklist => {
  const history = useHistory()

  const updatedQueryTrackFormEvents = history.whereType(updatedQueryTrackForm)
  const nextRequestId = useRequestIdGenerator()

  useEffect(() => {
    const lastUpdatedQueryTrackForm = didRequest(history) ?
      updatedQueryTrackFormEvents[updatedQueryTrackFormEvents.length - 1].payload : 
      fallbacks.queryForm
    const requestId = nextRequestId()
    history.save(
      initiatedTracksFetch({
        data: lastUpdatedQueryTrackForm,
        id: requestId
      })
    )
    if (lastUpdatedQueryTrackForm.dataSource === "MusicDb")
      tracksApi
        .fetchFromMusicDb({
          ...lastUpdatedQueryTrackForm.fields!,
          take: pageSize,
          skip: 0
        })
        .then(r =>
          history.save(fetchedTracksFromMusicDb({ data: r.data, requestId }))
        )
        .catch(() => history.save(fetchTracksFailed(requestId)))
    else
      tracksApi
        .fetchFromYouTube(lastUpdatedQueryTrackForm.searchQuery!)
        .then(r =>
          history.save(fetchedTracksFromYouTube({ data: r.data, requestId }))
        )
        .catch(() => history.save(fetchTracksFailed(requestId)))
  }, [updatedQueryTrackFormEvents.length])

  let fromMusicDb: Tracklist["fromMusicDb"]
  let fromYouTube: Tracklist["fromYouTube"]

  const queryForm = getLatestQueryForm(history) || fallbacks.queryForm

  if(didRequest(history)){
    if (queryForm.dataSource === "MusicDb")
      fromMusicDb = tryExtractMusicDbTracklistState(history)
    else fromYouTube = tryExtractYoutubeTracklistState(history)
  }  

  const setQueryForm = (form: TrackQueryForm) =>
    history.save(updatedQueryTrackForm(form))
  const saveTrack = (t: Track) => tracksApi.save(t)

  const fetchTracksNextPage = () => {
    const initialRequestId = getLastMusicDbFetchId(history)
    const skip = getNumberOfTracksFetched(history)
    const reqId = nextRequestId()
    tracksApi
      .fetchFromMusicDb({ ...queryForm.fields!, take: pageSize, skip })
      .then(r =>
        history.save(
          fetchedTracksNextPage({ initialRequestId, data: r.data, id: reqId })
        )
      )
      .then(r =>
        history.save(
          fetchTracksNextPageFailed({ initialRequestId, requestId: reqId })
        )
      )
  }

  const onTrackClick = (trackYoutubeId: string) => {
    history.save(selectedTrack(trackYoutubeId))
  }

  return {
    didRequest: didRequest(history),
    queryForm,
    fromMusicDb,
    fromYouTube,
    fetchTracksNextPage,
    setQueryForm,
    saveTrack,
    onTrackClick,
    selectedTrackId: getSelectedTrackYoutubeId(history)
  }
}
