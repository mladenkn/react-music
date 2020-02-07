import axios from "axios";
import { Track, MusicDbTrackQueryForm, Paging } from "./shared";
import { ArrayWithTotalCount } from "../utils/types";

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

export const fetchFromMusicDb = async (query: MusicDbTrackQueryForm & Paging) => {
  const r = await axios.get<{ data: ArrayWithTotalCount<Track>; totalCount: number }>(
    `${baseUrl}tracks`, { params: query }
  );
  return r;
};

export const fetchRelated = (
  videoId: string
): Promise<Track[]> => {
  throw new Error("fetchRelatedTracks not implemented");
};

export const save = async (data: Track) => {
  await axios.post(`${baseUrl}tracks`, data);
};

export const tracksApi = {
  fetchFromYouTube,
  fetchFromMusicDb,
  fetchRelated,
  save 
}