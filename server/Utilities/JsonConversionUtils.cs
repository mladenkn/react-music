using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Utilities
{
    public static class JsonConversionUtils
    {
        public static Dictionary<string, object> ToDictionary(this JObject json)
        {
            var propertyValuePairs = json.ToObject<Dictionary<string, object>>();
            ProcessJObjectProperties(propertyValuePairs);
            ProcessJArrayProperties(propertyValuePairs);
            return propertyValuePairs;
        }

        private static void ProcessJObjectProperties(IDictionary<string, object> propertyValuePairs)
        {
            var objectPropertyNames = propertyValuePairs
                .Select(property => new {property, propertyName = property.Key})
                .Select(t => new {t, value = t.property.Value})
                .Where(t => t.value is JObject)
                .Select(t => t.t.propertyName)
                .ToList();

            objectPropertyNames.ForEach(propertyName => propertyValuePairs[propertyName] = ToDictionary((JObject)propertyValuePairs[propertyName]));
        }

        private static void ProcessJArrayProperties(IDictionary<string, object> propertyValuePairs)
        {
            var arrayPropertyNames = propertyValuePairs
                .Select(property => new {property, propertyName = property.Key})
                .Select(t => new {t, value = t.property.Value})
                .Where(t => t.value is JArray)
                .Select(t => t.t.propertyName)
                .ToList();

            arrayPropertyNames.ForEach(propertyName => propertyValuePairs[propertyName] = ToArray((JArray)propertyValuePairs[propertyName]));
        }

        public static object ToArray(this JArray array)
        {
            if (array.AllElementsAreOfSameType())
            {
                var type = array[0].Type;
                switch (type)
                {
                    case JTokenType.Boolean:
                        return array.Select(e => e.Value<bool>()).ToArray();
                    case JTokenType.Float:
                        return array.Select(e => e.Value<float>()).ToArray();
                    case JTokenType.Integer:
                        return array.Select(e => e.Value<int>()).ToArray();
                    case JTokenType.String:
                        return array.Select(e => e.Value<string>()).ToArray();
                    //case JTokenType.Array:
                    //    var firstElement = prop.Values<JToken>().FirstOrDefault();
                    //    if (firstElement == null)
                    //        throw new Exception("Empty arrays not supported");
                    //    var mapped = prop.Values<JToken>().Select(GetPropertyValue).ToArray();
                    //    switch (firstElement.Type)
                    //    {
                    //        case JTokenType.Boolean:
                    //            return (object)mapped.Cast<bool>();
                    //        case JTokenType.Float:
                    //            return mapped.Cast<float>();
                    //        case JTokenType.Integer:
                    //            return mapped.Cast<int>();
                    //        case JTokenType.String:
                    //            return mapped.Cast<string>();
                    //        default:
                    //            throw new Exception("Parameter type not supported");
                    //    }
                    default:
                        throw new Exception("Parameter type not supported");
                }
            }
            else
                return array.ToObject<object[]>().Select(ProcessArrayEntry).ToArray();
        }

        public static bool AllElementsAreOfSameType(this JArray array) => array.DistinctBy(e => e.Type).Count() == 1;

        private static object ProcessArrayEntry(object value)
        {
            if (value is JObject jObject)
                return ToDictionary(jObject);
            else if (value is JArray jArray)
                return ToArray(jArray);
            return value;
        }
    }
}