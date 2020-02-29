using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using Music.App.DbModels;
using Utilities;

namespace Music.App
{
    public class DataPersistor : ServiceResolverAware
    {
        private readonly MusicDbContext _db;

        public DataPersistor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = serviceProvider.GetRequiredService<MusicDbContext>();
        }

        public new async Task Persist(Action<DataPersistorOperations> specifyOperations)
        {
            var actions = new List<Action>();
            var ops = new DataPersistorOperations(_db, actions.Add);
            specifyOperations(ops);
            await Db.SaveChangesAsync();
            actions.ForEach(a => a());
        }
    }

    public class DataPersistorOperations
    {
        private readonly MusicDbContext _db;
        private readonly Action<Action> _afterCommitTransaction;

        public DataPersistorOperations(MusicDbContext db, Action<Action> afterCommitTransaction)
        {
            _db = db;
            _afterCommitTransaction = afterCommitTransaction;
        }

        public void InsertTracks(IEnumerable<Track> tracks)
        {
            var pairs = Copy(tracks, t =>
            {
                t.YoutubeVideos = null;
                t.TrackUserProps = null;
            }).ToArray();
            _db.Tracks.AddRange(pairs.Select(t => t.copy));
            _afterCommitTransaction(
                () => pairs.ForEach(p => p.original.Id = p.copy.Id)
            );
        }

        public void InsertTrackUserProps(IEnumerable<TrackUserProps> trackUserProps)
        {
            var pairs = Copy(trackUserProps, item =>
            {
                item.Track = null;
                item.User = null;
                item.YoutubeVideo = null;
            }).ToArray();

            _db.TrackUserProps.AddRange(pairs.Select(p => p.copy));
            _afterCommitTransaction(
                () => pairs.ForEach(p => p.original.Id = p.copy.Id)
            );
        }

        public void UpdateTrackUserProps(IEnumerable<TrackUserProps> trackUserProps)
        {
            var readyToUpdate = Copy(trackUserProps, t =>
            {
                t.Track = null;
                t.User = null;
                t.YoutubeVideo = null;
            });
            _afterCommitTransaction(
                () => _db.TrackUserProps.UpdateRange(readyToUpdate.Select(i => i.original))
            );
        }

        public void InsertTrackUserPropsTags(IEnumerable<TrackUserPropsTag> models) => 
            _db.TrackUserPropsTags.AddRange(models);

        public void DeleteTrackUserPropsTags(IEnumerable<TrackUserPropsTag> models) => 
            _db.TrackUserPropsTags.RemoveRange(models);

        public void InsertYouTubeVideos(IEnumerable<YoutubeVideo> videos)
        {
            var pairs = Copy(videos, v => v.YouTubeChannel = null).ToArray();
            _db.YoutubeVideos.AddRange(pairs.Select(p => p.copy));
            _afterCommitTransaction(
                () => pairs.ForEach(p => p.original.Id = p.copy.Id)
            );
        }

        public void InsertUser(IEnumerable<User> users) => _db.Users.AddRange(users);

        public void UpdateUsers(IEnumerable<User> users) => _db.Users.UpdateRange(users);

        public void InsertYouTubeChannels(IEnumerable<YouTubeChannel> channels) =>
            _db.YouTubeChannels.AddRange(channels);

        private IEnumerable<(T original, T copy)> Copy<T>(IEnumerable<T> source, Action<T> mutate) where T : new()
        {
            var pairs = source.Select(original =>
            {
                var copy = ReflectionUtils.ShallowCopy(original);
                mutate(copy);
                return (original, copy);
            });
            return pairs;
        }
    }
}
