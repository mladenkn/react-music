import {
  TrackQueryData,
  TrackData,
  LoadedTracksResponse,
  YoutubeTrackQuery,
  WithUserPermissions
} from "../dataModels";
import axios from "axios";

let baseUrl: string;

if (process.env.NODE_ENV === "development")
  baseUrl = "https://localhost:44365/api/";
else if (process.env.NODE_ENV === "production")
  baseUrl = window.location.href + "api/";
else throw new Error();

export const fetchTracksFromYT = (q: YoutubeTrackQuery) =>
  get<{ tracks: TrackData[] }>(baseUrl + "tracks-yt/", { query: q });

export const fetchTracks = async (query: TrackQueryData) => {
  const queryEntries = Object.entries(query).filter(([key, value]) => {
    if(value === null  ||  value === undefined)
        return false;
    else if (Array.isArray(value) && value.length === 0)
        return false;
    else if (typeof value === 'object' && Object.entries(value).length === 0)
        return false;
    else
        return true;
  })
  const query_ = Object.fromEntries(queryEntries);
  debugger;
  const r = await get<LoadedTracksResponse>(`${baseUrl}tracks`, query_);
  return r;
};

export const fetchRelatedTracks = (videoID: string) =>
  get<{ tracks: TrackData[] }>(`${baseUrl}tracks/recommendations/`, {
    videoID
  });

export const saveTracks = (data: TrackData[]) => {
  console.log(data);
  return post<{
    tracks: TrackData[];
  }>(`${baseUrl}tracks`, { tracks: data });
};

const get = <TResponse>(url: string, params: unknown) => {
  const userKey = getUserKey();
  return axios
    .get<TResponse & WithUserPermissions>(url, {
      params: { ...params, userKey }
    })
    .then(r => r.data);
};

const post = <TResponse>(url: string, params: unknown) => {
  const userKey = getUserKey();
  return axios
    .post<TResponse & WithUserPermissions>(url, { ...params, userKey })
    .then(r => r.data);
};

const getUserKey = () =>
  (localStorage.getItem("userKey") || undefined) as string | undefined;
