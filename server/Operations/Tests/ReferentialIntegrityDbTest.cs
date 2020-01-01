using System;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Executables.Tests
{
    public class ReferentialIntegrityDbTest
    {
        private readonly DataGenerator _gen = new DataGenerator();
        private readonly string _dbName = Guid.NewGuid().ToString();

        [Fact]
        public async Task Should_Fail_Trying_To_Add_Models_Because_Of_Foreign_key_constraint_violations()
        {
            var models = new (object model, string errorMessageContains)[]
            {
                (
                    model: _gen.TrackTag(t => { t.TrackUserPropsId = _gen.Int(); }),
                    errorMessageContains: "FK_TrackUserPropsTags_TrackUserProps_TrackUserPropsId"
                ),
                (
                    model: _gen.Track(t =>
                    {
                        t.YoutubeVideoId = "1";
                        t.User = _gen.User();
                    }),
                    errorMessageContains: "FK_TrackUserProps_YoutubeVideos_YoutubeVideoId"
                ),
                (
                    model: _gen.Track(t =>
                    {
                        t.UserId = 1;
                        t.YoutubeVideo = _gen.YoutubeVideo(v =>
                        {
                            v.Id = _gen.String();
                            v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                        });
                    }),
                    errorMessageContains: "FK_TrackUserProps_Users_UserId"
                ),
            };

            await using (var db = Utils.UseDbContext(_dbName))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                foreach (var model in models)
                {
                    db.Add(model.model);
                    FailAddingOne(model.model, model.errorMessageContains);
                }

                void FailAddingOne(object model, string errorMsgContains)
                {
                    //db.Add(model);
                    //try
                    //{
                    //    await db.SaveChangesAsync();
                    //}
                    //catch (DbUpdateException e)
                    //{
                    //}

                    db.Add(model);
                    db.Invoking(_ => _.SaveChanges())
                        .Should()
                        .Throw<DbUpdateException>()
                        .Where(e => e.InnerException.Message.Contains(errorMsgContains));

                    var entries = db.ChangeTracker.Entries();
                    foreach (var entry in entries)
                        entry.State = EntityState.Detached;
                }

                await db.Database.EnsureDeletedAsync();
            }
        }
    }
}
