using AutoMapper;
using Music.Domain.Shared;
using Xunit;

namespace Executables.Tests
{
    public class AutoMapperProfilesTest
    {
        [Fact]
        public void Run()
        {
            var mapperConfig = new MapperConfiguration(c => c.AddMaps(typeof(TrackModel).Assembly));
            mapperConfig.AssertConfigurationIsValid();
        }
    }
}
