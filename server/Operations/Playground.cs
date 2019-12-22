using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Executables
{
    public class Playground
    {
        [Fact]
        public async Task Run()
        {
            var a = new []
            {
                "PT11M", "PT11S", "PT11H",
                "PT11H41S", "PT11M41S", "PT11H41M",
                "PT11H41M34S",
                "PT1M", "PT1S", "PT1H",
                "PT1H41S", "PT11M41S", "PT1H4M",
                "PT1H4M3S",
            }
            .Select(XmlConvert.ToTimeSpan);
        }
    }
}
