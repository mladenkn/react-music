using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;
using Music.DataAccess.Models;
using Xunit;

namespace Executables
{
    public class DbTest
    {
        [Fact]
        public async Task Can_Add_One_YoutubeChannel()
        {
            var channel = new YoutubeChannel
            {
                Id = "1",
                Title = "channel"
            };

            var services = new Services();
            var db = services.DbContext;
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            db.Add(channel);
            await db.SaveChangesAsync();
            
            var channelQueryResult = await db.Set<YoutubeChannel>().ToArrayAsync();
            Assert.Single(channelQueryResult);
            Assert.Equal(channel.Id, channelQueryResult[0].Id);
            Assert.Equal(channel.Title, channelQueryResult[0].Title);
        }

        [Fact]
        public async Task Cannot_Add_TrackUserPropsTag_When_No_Such_TrackUserProps_Exists()
        {
            var services = new Services();
            var db = services.DbContext;
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            var trackUserPropsTag = new TrackUserPropsTag
            {
                TrackUserPropsId = 1,
                Value = "tag"
            };

            db.Add(trackUserPropsTag);
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        [Fact]
        public async Task Cannot_Add_TrackUserProps_When_No_Such_YoutubeVideo_Exists()
        {
            var services = new Services();
            var db = services.DbContext;
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            var user = new User
            {
                Email = "user@gmail.com"
            };

            db.Add(user);
            await db.SaveChangesAsync();

            var trackUserProps = new TrackUserProps
            {
                UserId = 1,
                YoutubeVideoId = "1",
            };

            db.Add(trackUserProps);
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
}
