import {
  TrackQueryData,
  TrackData,
  YoutubeTrackQuery,
  UserPermissions
} from "../dataModels";
import axios from "axios";

let baseUrl: string;

if (process.env.NODE_ENV === "development")
  baseUrl = "https://localhost:44365/api/";
else if (process.env.NODE_ENV === "production")
  baseUrl = window.location.href + "api/";
else throw new Error();

const allPermissions = {
  canEditTrackData: true,
  canFetchTrackRecommendations: true
};

const withAllPermissions = <T>(o: T) => ({
  ...o,
  permissions: allPermissions
});

export const fetchTracksFromYT = async (q: YoutubeTrackQuery) => {
  const r = await axios.get<TrackData[]>(`${baseUrl}tracks/yt`, {
    params: { searchQuery: q.searchQuery }
  });
  return withAllPermissions({ tracks: r.data });
};

export const fetchTracks = async (query: TrackQueryData) => {
  var query_ = {
    skip: query.skip,
    take: query.take,
    titleContains: query.titleMatch,
    mustHaveEveryTag: query.mustContainAllTags,
    mustHaveAnyTag: query.mustContainAllTags,
    yearRange: {
      from: query.yearSpan && query.yearSpan.from,
      to: query.yearSpan && query.yearSpan!.to
    }
  };
  const r = await axios.get<{ data: TrackData[]; totalCount: number }>(
    `${baseUrl}tracks`,
    { params: query_ }
  );
  return withAllPermissions(r.data);
};

export const fetchRelatedTracks = (
  videoID: string
): Promise<{ tracks: TrackData[]; permissions: UserPermissions }> => {
  throw new Error("fetchRelatedTracks not implemented");
};

export const saveTracks = async (data: TrackData[]) => {
  const firstTrackMapped = {
    trackYtId: data[0].ytId,
    tags: data[0].tags,
    year: data[0].year
  };
  await axios.post(`${baseUrl}tracks`, firstTrackMapped);
  return allPermissions;
};

// const get = <TResponse>(url: string, params: object) => {
//   const userKey = getUserKey();
//   return axios
//     .get<TResponse & WithUserPermissions>(url, {
//       params: { ...params, userKey }
//     })
//     .then(r => r.data);
// };

// const post = <TResponse>(url: string, params: object) => {
//   const userKey = getUserKey();
//   return axios
//     .post<TResponse & WithUserPermissions>(url, { ...params, userKey })
//     .then(r => r.data);
// };

// const post2 = (url: string, params: object) => {
//   const userKey = getUserKey();
//   return axios
//     .post<UserPermissions>(url, { ...params, userKey })
//     .then(r => r.data);
// };

// const getUserKey = () =>
//   (localStorage.getItem("userKey") || undefined) as string | undefined;
