using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Music.Admin.Services
{
    public class YamlService : ServiceResolverAware
    {
        public YamlService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private readonly ISerializer _serializer = new SerializerBuilder().Build();
        private readonly IDeserializer _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public IReadOnlyDictionary<string, object> DeserializeToDictionary(string yaml)
        {
            var o = _deserializer.Deserialize<Dictionary<string, object>>(new StringReader(yaml));
            return o;
        }

        public string Serialize(IReadOnlyDictionary<string, object> dict) => _serializer.Serialize(dict);
    }
}
