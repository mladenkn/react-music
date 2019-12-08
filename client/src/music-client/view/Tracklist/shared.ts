import { TrackData as TrackData_ } from "../../../dataModels";
import { Omit } from 'utility-types';

export type TrackData = Omit<TrackData_, 'ytID'> & {id: string, canPlay: boolean, canEdit: boolean}