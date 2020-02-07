import { useTracklist as useTracklist_, Tracklist } from "./tracklist"

export const useTracklist = () => {
  const wrapped = useTracklist_()
  return {
    ...wrapped,
    fromMusicDb: mapTracksFromMusicDb(wrapped.fromMusicDb),
    fromYouTube: mapTracksFromYouTube(wrapped.fromYouTube)
  }
}

const mapTracksFromMusicDb = (data: Tracklist["fromMusicDb"]) => {
  if (!data) return undefined
  return {
    list: data.list && {
      data: data.list.data.map(t => ({ ...t })),
      totalCount: data.list.totalCount
    },
    status: data.status
  }
}

const mapTracksFromYouTube = (data: Tracklist["fromYouTube"]) => {
  if (!data) return undefined
  return {
    list: data.list && data.list.map(t => ({ ...t })),
    status: data.status
  }
}
