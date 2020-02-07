export interface Track {
  youtubeVideoId: string;
  title: string;
  image: string;
  description: string;
  youtubeChannelId: string;
  year: number;
  tags: string[];
}

export interface TrackViewModel extends Track {
  discogsSearchUrl: string
  youtubeVideoUrl: string
  isSelected: boolean
  
}

export interface SaveTrackModel {
  trackYtId: string;
  tags: string[];
  year: number;
}

export interface Range<T> {
  lowerBound: T;
  upperBound: T;
}
