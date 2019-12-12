using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Models;
using Music.Repositories;

namespace Music.Services
{
    public class TrackService
    {
        private readonly TrackRepository _repo;

        public TrackService(TrackRepository repo)
        {
            _repo = repo;
        }

        public Task<GetTrackListResponse> GetList() => _repo.GetList();

        //public async Task<GetTrackListResponse> GetList()
        //{
        //    var allTracks = await GetListFromDb();
        //    var allTrackIds = allTracks.Select(t => (string) t.Id).ToList();
        //    var allTracksFromYt = await _youtubeTrackRepo.GetList(allTrackIds);

        //    var tracksMapped = allTracks.Select(trackFromDb =>
        //    {
        //        var trackFromYt = allTracksFromYt.Single(t => t.Id == trackFromDb.Id);
        //        Track trackMapped = Create(trackFromYt, trackFromDb);
        //        return trackMapped;
        //    });

        //    return new GetTrackListResponse
        //    {
        //        Data = tracksMapped,
        //        ThereIsMore = false,
        //        TotalCount = allTrackIds.Count,
        //    };
        //}

        //private async Task<IEnumerable<dynamic>> GetListFromDb()
        //{
        //    var trackCursor = await _tracksCollection.FindAsync(t => true);
        //    var allTracks = (await trackCursor.ToListAsync()).Take(50);

        //    return allTracks.Select(trackFromDb => new
        //    {
        //        Id = trackFromDb.GetValue("ytID").AsString,
        //        Year = ExtractYear(trackFromDb),
        //        Tags = trackFromDb.GetValue("tags").AsBsonArray.ToArray().Select(i => i.AsString),
        //        Genres = trackFromDb.GetValue("genres").AsBsonArray.ToArray().Select(i => i.AsString),
        //    });
        //}

        //private int? ExtractYear(BsonDocument track)
        //{
        //    if (!track.ContainsValue("year"))
        //        return null;
        //    else
        //    {
        //        var yearBson = track.GetValue("year");
        //        if (yearBson.IsInt32)
        //            return yearBson.AsInt32;
        //        else if (yearBson.IsString)
        //            return int.Parse(yearBson.AsString);
        //        else
        //        {
        //            throw new Exception("Unsupported type for Track.Year");
        //        }
        //    }
        //}

        //private static Track Create(YoutubeVideo fromYt, dynamic fromDb) =>
        //    new Track
        //    {
        //        YtId = fromYt.Id,
        //        Title = fromYt.Title,
        //        Image = fromYt.Image,
        //        Description = fromYt.Description,
        //        Channel = new TrackChannel
        //        {
        //            Id = fromYt.ChannelId,
        //            Title = fromYt.ChannelTitle,
        //        },
        //        Year = fromDb.Year,
        //        Genres = fromDb.Genres,
        //        Tags = fromDb.Tags
        //    };
    }
}