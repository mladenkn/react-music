import axios from "axios";
import { Paging } from "../shared";
import { ArrayWithTotalCount } from "../../utils/types";
import { MusicDbTrackQueryParams } from "../shared/homeSectionOptions";
import { Track, SaveTrackModel } from "../shared/track";
import qs from 'qs'

let baseUrl: string;

if (process.env.NODE_ENV === "development")
  baseUrl = "https://localhost:44365/api/";
else if (process.env.NODE_ENV === "production")
  baseUrl = window.location.href + "api/";
else throw new Error();

export const fetchFromYouTube = async (searchQuery: string) => {
  const r = await axios.get<Track[]>(`${baseUrl}tracks/yt`, {
    params: { searchQuery }
  });
  return r;
};

export const fetchFromMusicDb = async (params: MusicDbTrackQueryParams & Paging) => {
  const r = await axios.get<ArrayWithTotalCount<Track>>(
    `${baseUrl}tracks`,
    { 
      params, 
      paramsSerializer: params => qs.stringify(params, { allowDots: true }) 
    }
  );
  return r;
};

export const fetchRelated = (
  videoId: string
): Promise<Track[]> => {
  throw new Error("fetchRelatedTracks not implemented");
};

export const save = (data: SaveTrackModel, query?: MusicDbTrackQueryParams & Paging) => {
  return axios.post<ArrayWithTotalCount<Track>>(`${baseUrl}tracks`, { ...data, query });
};

export const homeSectionApi = {
  fetchFromYouTube,
  fetchFromMusicDb,
  fetchRelated,
  save 
}