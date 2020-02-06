import { TrackQueryForm } from "./trackQueryForm";
import { Track } from "../shared";
import { tracksApi } from "../apiClient";
import { useEffect } from "react";
import { ArrayWithTotalCount, AsyncOperationStatus } from "../../utils/types";
import { useHistory } from "../../utils/es/history";
import {
  updatedQueryTrackForm,
  initiatedTracksFetch,
  fetchTracksFailed,
  fetchedTracksFromMusicDb,
  fetchedTracksFromYouTube
} from "./homeSection.events";
import { useRequestIdGenerator } from "./requestIdGenerator";

export interface HomeSection {
  trackQueryForm: TrackQueryForm;
  tracksFromMusicDb?: {
    data?: ArrayWithTotalCount<Track[]>;
    status: AsyncOperationStatus;
  };
  tracksFromYouTube?: {
    data?: Track[];
    status: AsyncOperationStatus;
  };
  setTrackQueryForm(trackQueryForm: TrackQueryForm): void;
  saveTrack(t: Track): Promise<void>;
}

const pageSize = 20;

export const useHomeSection = (): HomeSection => {
  const history = useHistory();

  const updatedQueryTrackFormEvents = history.whereType(updatedQueryTrackForm);
  const nextRequestId = useRequestIdGenerator();

  useEffect(() => {
    const lastUpdatedQueryTrackForm =
      updatedQueryTrackFormEvents[updatedQueryTrackFormEvents.length - 1];
    const requestId = nextRequestId();
    history.save(initiatedTracksFetch({ data: lastUpdatedQueryTrackForm.payload, id: requestId }));

    if (lastUpdatedQueryTrackForm.payload.dataSource === "MusicDb")
      tracksApi
        .fetchFromMusicDb({
          ...lastUpdatedQueryTrackForm.payload.fields!,
          take: pageSize,
          skip: 0
        })
        .then(r => history.save(fetchedTracksFromMusicDb({ data: r.data, requestId })))
        .catch(() => history.save(fetchTracksFailed(requestId)));
    else
      tracksApi
        .fetchFromYT(lastUpdatedQueryTrackForm.payload.searchQuery!)
        .then(r => history.save(fetchedTracksFromYouTube({ data: r.data, requestId })))
        .catch(() => history.save(fetchTracksFailed(requestId)));

  }, [updatedQueryTrackFormEvents.length]);

  const setTrackQueryForm = (trackQueryForm: TrackQueryForm) => {
    history.save(updatedQueryTrackForm(trackQueryForm));
  };

  const saveTrack = (t: Track) => tracksApi.save(t);

  throw new Error("Not implemented.");
};
