import axios from "axios";
import { MusicDbTrackQueryForm } from "./logic/trackQueryForm";
import { Track } from "./shared";

let baseUrl: string;

if (process.env.NODE_ENV === "development")
  baseUrl = "https://localhost:44365/api/";
else if (process.env.NODE_ENV === "production")
  baseUrl = window.location.href + "api/";
else throw new Error();

export const fetchTracksFromYT = async (searchQuery: string) => {
  const r = await axios.get<Track[]>(`${baseUrl}tracks/yt`, {
    params: { searchQuery }
  });
  return r.data;
};

export const fetchTracks = async (query: MusicDbTrackQueryForm) => {
  const r = await axios.get<{ data: Track[]; totalCount: number }>(
    `${baseUrl}tracks`, { params: query }
  );
  return r.data;
};

export const fetchRelatedTracks = (
  videoId: string
): Promise<Track[]> => {
  throw new Error("fetchRelatedTracks not implemented");
};

export const saveTracks = async (data: Track) => {
  await axios.post(`${baseUrl}tracks`, data);
};