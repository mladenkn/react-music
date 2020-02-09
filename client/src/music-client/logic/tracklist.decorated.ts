import { useTracklistLogic as useTracklistLogic_, Tracklist } from "./tracklist"
import { Track, TrackViewModel } from "../shared"

export const useTracklistLogic = () => {
  const wrapped = useTracklistLogic_()

  const isLoading = 
    (wrapped.fromMusicDb && wrapped.fromMusicDb.status === 'PROCESSING') ||
    (wrapped.fromYouTube && wrapped.fromYouTube.status === 'PROCESSING')

  const isError = 
    (wrapped.fromMusicDb && wrapped.fromMusicDb.status === 'ERROR') ||
    (wrapped.fromYouTube && wrapped.fromYouTube.status === 'ERROR')

  const isLoaded = 
    (wrapped.queryForm.dataSource === 'MusicDb' && wrapped.fromMusicDb && wrapped.fromMusicDb!.status === 'PROCESSED') ||
    (wrapped.queryForm.dataSource === 'YouTube' && wrapped.fromYouTube && wrapped.fromYouTube!.status === 'PROCESSED')

  return {
    ...wrapped,
    isLoaded,
    isLoading,
    isError,
    fromMusicDb: mapTracksFromMusicDb(wrapped.fromMusicDb, wrapped.selectedTrackId),
    fromYouTube: mapTracksFromYouTube(wrapped.fromYouTube, wrapped.selectedTrackId)
  }
}

const mapTracksFromMusicDb = (data: Tracklist["fromMusicDb"], selectedTrackYoutubeId?: string) => {
  if (!data) return undefined
  return {
    list: data.list && {
      data: data.list.data.map(t => mapTrack(t, selectedTrackYoutubeId)),
      totalCount: data.list.totalCount
    },
    status: data.status
  }
}

const mapTracksFromYouTube = (data: Tracklist["fromYouTube"], selectedTrackYoutubeId?: string) => {
  if (!data) return undefined
  return {
    list: data.list && data.list.map(t => mapTrack(t, selectedTrackYoutubeId)),
    status: data.status
  }
}

const mapTrack = (t: Track, selectedTrackYoutubeId?: string): TrackViewModel => ({
  discogsSearchUrl: '',
  youtubeVideoUrl: '',
  canEdit: true,
  canFetchRecommendations: true,
  canPlay: true,
  ...t,
  editableProps: {
    year: t.year,
    tags: t.tags
  },
  isSelected: t.youtubeVideoId == selectedTrackYoutubeId
})