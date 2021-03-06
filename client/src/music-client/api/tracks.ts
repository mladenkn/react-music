import { Paging } from "../shared";
import { ArrayWithTotalCount } from "../../utils/types";
import { MusicDbTrackQueryParams, HomeSectionOptions } from "../shared/homeSectionOptions";
import { Track, SaveTrackModel } from "../shared/track";
import qs from 'qs'
import { useAxios } from "./axios";

export const useTracksApi = () => {

  const { get, post, patch } = useAxios()

  const fetchFromYouTube = async (searchQuery: string) => {
    const r = await get<Track[]>('tracks/yt', { params: { searchQuery } });
    return r;
  };
  
  const fetchFromMusicDb = async (params: MusicDbTrackQueryParams & Paging) => {
    const r = await get<ArrayWithTotalCount<Track>>(
      'tracks',
      { 
        params, 
        paramsSerializer: params => qs.stringify(params, { allowDots: true }) 
      }
    );
    return r;
  };
  
  const save = (data: SaveTrackModel) => {
    return post<ArrayWithTotalCount<Track>>('tracks', data);
  };
  
  const declareANonTrack = (videoId: string) => {
    return patch(`tracks/declareANonTrack/${videoId}`)
  }

  return { fetchFromYouTube, fetchFromMusicDb, save, declareANonTrack }
}
