export interface Track {
  youtubeVideoId: string
  title: string
  image: string
  description: string
  youtubeChannelId: string
  youtubeChannelTitle: string
  year: number
  tags: string[]
}

export interface TrackViewModel extends Track {
  discogsSearchUrl: string
  youtubeVideoUrl: string
  isSelected: boolean
  editableProps: TrackEditableProps
  canFetchRecommendations: boolean
  canEdit: boolean
  canPlay: boolean
}

export interface SaveTrackModel {
  trackYtId: string
  tags: string[]
  year: number
}

export interface TrackEditableProps {
  year: number
  tags: string[]
}

export const mapToTrackViewModel = (t: Track, selectedTrackYoutubeId?: string): TrackViewModel => ({
  discogsSearchUrl: `https://www.discogs.com/search/?q=${t.title}&type=all`,
  youtubeVideoUrl: `https://www.youtube.com/watch?v=${t.youtubeVideoId}`,
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