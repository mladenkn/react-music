import { TrackViewModel, Track } from "../../shared";

export const mapToTrackViewModel = (t: Track, selectedTrackYoutubeId?: string): TrackViewModel => ({
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