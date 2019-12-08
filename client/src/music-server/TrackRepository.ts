import { MongoBaseRepository } from "../utils";
import { Db, FilterQuery } from "mongodb";
import { TrackData, TrackQueryData, LoadedTracksResponse } from "../dataModels";
import { dissoc, assoc } from "ramda";
import { set } from "lodash";

export class TrackRepository extends MongoBaseRepository<TrackData> {
  constructor(db: Db) {
    super(db, "tracks");
  }

  getAll(): Promise<TrackData[]> {
    return this.collection.find({}).toArray();
  }

  buildFilter(query: TrackQueryData) {
    let filter: FilterQuery<TrackData> = {};

    if (query.mustContainAllGenres && query.mustContainAllGenres.length !== 0)
      set(filter, "genres.$all", query.mustContainAllGenres);

    if (
      query.mustContainSomeGenres &&
      query.mustContainSomeGenres.length !== 0
    ) {
    }

    if (query.mustContainAllTags && query.mustContainAllTags.length !== 0)
      set(filter, "tags.$all", query.mustContainAllTags);

    if (query.mustContainSomeTags && query.mustContainSomeTags.length !== 0) {
    }

    if (query.yearSpan && Object.entries(query.yearSpan).length !== 0) {
      if (query.yearSpan!.from) set(filter, "year.$gte", query.yearSpan!.from);
      if (query.yearSpan!.to) set(filter, "year.$lt", query.yearSpan!.to);
    }

    if (query.titleMatch) filter.title = { $regex: query.titleMatch };

    if (query.channel) filter["channel.title"] = { $regex: query.channel };

    return filter;
  }

  async getList(query: TrackQueryData): Promise<LoadedTracksResponse> {
    console.log("TrackRepository.getList begin");
    let filter = this.buildFilter(query);

    const totalCount = await this.collection.find(filter).count();
    const data = (await this.collection
      .find(filter)
      .skip(query.skip)
      .limit(query.take)
      .sort({ savedAt: -1 })
      .map(dissoc("savedAt"))
      .toArray()) as TrackData[];
    const thereIsMore = data.length < totalCount;

    console.log(query);
    console.log(filter);
    console.log("TrackRepository.getList end");

    return { data, thereIsMore, totalCount };
  }

  getWithIds(ids: string[]): Promise<TrackData[]> {
    return this.collection.find({ ytID: { $in: ids } }).toArray();
  }

  async save(tracks: TrackData[]) {
    for (const track of tracks) {
      const trackWithoutId = dissoc("_id", track);
      const exists = !!(await this.collection.findOne({ ytID: track.ytId }));
      if (exists)
        await this.collection.updateOne(
          { ytID: track.ytId },
          { $set: trackWithoutId }
        );
      else {
        const withSavedAt = assoc("savedAt", new Date(), trackWithoutId);
        await this.collection.insertOne(withSavedAt);
      }
    }
  }
}
