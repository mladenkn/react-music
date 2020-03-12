using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void InsertTracks(IEnumerable<Track> tracks, Action<Track> mutate)
        {
            var pairs = Copy(tracks, mutate).ToArray();
            _db.Tracks.AddRange(pairs.Select(p => p.copy));
            _afterCommitTransaction(
                () =>
                {
                    pairs.ForEach(p => p.original.Id = p.copy.Id);
                });
        }

        public void InsertTrackUserProps(IEnumerable<TrackUserProps> trackUserProps, Action<TrackUserProps> mutate = null)
        {
            var pairs = Copy(trackUserProps, mutate).ToArray();
            _db.TrackUserProps.AddRange(pairs.Select(p => p.copy));
            _afterCommitTransaction(
                () => pairs.ForEach(p => p.original.Id = p.copy.Id)
            );
        }

        public void UpdateTrackUserProps(IEnumerable<TrackUserProps> trackUserProps, Action<TrackUserProps> mutate = null)
        {
            var pairs = Copy(trackUserProps.ToArray(), mutate).ToArray();
            _db.TrackUserProps.UpdateRange(pairs.Select(p => p.copy));
            _afterCommitTransaction(
                () => pairs.ForEach(p => p.original.Id = p.copy.Id)
            );
        }

        public void InsertTrackUserPropsTags(IEnumerable<TrackUserPropsTag> models) => 
            _db.TrackUserPropsTags.AddRange(models);

        public void DeleteTrackUserPropsTags(IEnumerable<TrackUserPropsTag> models) => 
            _db.TrackUserPropsTags.RemoveRange(models);

        public void InsertYouTubeVideos(IEnumerable<YoutubeVideo> videos, Action<YoutubeVideo> mutate = null)
        {
            var pairs = Copy(videos, mutate).ToArray();
            _db.YoutubeVideos.AddRange(pairs.Select(p => p.copy));
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
                mutate?.Invoke(copy);
                return (original, copy);
            });
            return pairs;
        }
    }
}
