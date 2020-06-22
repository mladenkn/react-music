using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Music.Services
{
    public partial class AdminService
    {
        public async Task<object> ExecuteCommand(JObject cmd)
        {
            var type = cmd.GetValue("type")!.Value<string>();

            async Task<object> Execute()
            {
                switch (type)
                {
                    case "AddTracksToYouTubeVideos":
                    {
                        var videoIds = cmd.GetValue("videoIds")!.Values<string>();
                        return await Resolve<YouTubeVideosService>().AddTracksToVideos(videoIds);
                    }

                    case "DeleteTracks":
                    {
                        var trackIds = cmd.GetValue("tracks")!.Values<long>().ToArray();
                        await Resolve<TracksService>().Delete(trackIds);
                        return "Successfully deleted all stated tracks";
                    }
                    case "CallMethod":
                    {
                        var methodParam = cmd.GetValue("method")!.Value<string>();
                        var methodParamIndexOfDot = methodParam.IndexOf(".")!;

                        var className = methodParam.Substring(0, methodParamIndexOfDot);
                        var methodName = methodParam.Substring(methodParamIndexOfDot + 1);
                        var params_ = ((JObject) cmd.GetValue("params"))?.Properties().ToArray() ?? new JProperty[0];

                        var classTypeInfo = GetClass(className);
                        var matchingMethods = GetMatchingMethods(classTypeInfo, methodName, params_).ToArray();

                        if (matchingMethods.Length == 0)
                            throw new ApplicationException("No matching methods found");
                        if (matchingMethods.Length != 1)
                            throw new ApplicationException($"Found {matchingMethods.Length} matching methods");

                        var classInstance = _serviceProvider.GetService(classTypeInfo);
                        var result = await CallMethod(classInstance, matchingMethods.Single(), params_);

                        return result;
                    }
                    default:
                        return "Unsupported command";
                }
            }

            if (type == null)
                throw new ApplicationException();
            else
            {
                try
                {
                    var r = await Execute();
                    return r;
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException($"Command failed to execute because of user's mistake. {e.Message}");
                }
            }
        }

        private TypeInfo GetClass(string name)
        {
            var assembly = typeof(AdminService).Assembly;
            var classTypeInfo = assembly.DefinedTypes.Single(t => t.Name == name);
            return classTypeInfo;
        }

        private IEnumerable<MethodInfo> GetMatchingMethods(TypeInfo classTypeInfo, string methodName, IReadOnlyList<JProperty> params_)
        {
            return classTypeInfo.DeclaredMethods.Where(m =>
            {
                var parametersInfo = m.GetParameters();
                return m.Name == methodName && 
                       parametersInfo.Length == params_.Count() &&
                       DoParamsMatch(parametersInfo, params_);
            });
        }

        private bool DoParamsMatch(IReadOnlyList<ParameterInfo> methodParams, IReadOnlyList<JProperty> params_)
        {
            var index = 0;
            foreach (var property in params_)
            {
                var paramInfo = methodParams[index];
                if (paramInfo.Name != property.Name)
                    return false;
                index++;
            }

            return true;
        }

        private async Task<object> CallMethod(object classInstance, MethodInfo method, IReadOnlyList<JProperty> params_)
        {
            var paramsTransformed = params_.Select(e => GetPropertyValue(e.Value)).ToArray();
            var result = method.Invoke(classInstance, paramsTransformed);
            switch (result)
            {
                case null:
                    return null;
                case Task taskResult:
                {
                    await taskResult;
                    return result.GetType().IsGenericType ? 
                        result.GetType().GetProperty("Result")!.GetValue(result) : 
                        null;
                }
                default:
                    return result;
            }
        }

        private object GetPropertyValue(JToken prop)
        {
            switch (prop.Type)
            {
                case JTokenType.Boolean:
                    return (object)prop.Value<bool>();
                case JTokenType.Float:
                    return prop.Value<float>();
                case JTokenType.Integer:
                    return prop.Value<int>();
                case JTokenType.String:
                    return prop.Value<string>();
                case JTokenType.Array:
                    var firstElement = prop.Values<JToken>().FirstOrDefault();
                    if (firstElement == null)
                        throw new Exception("Empty arrays not supported");
                    var mapped = prop.Values<JToken>().Select(GetPropertyValue).ToArray();
                    switch (firstElement.Type)
                    {
                        case JTokenType.Boolean:
                            return (object)mapped.Cast<bool>();
                        case JTokenType.Float:
                            return mapped.Cast<float>();
                        case JTokenType.Integer:
                            return mapped.Cast<int>();
                        case JTokenType.String:
                            return mapped.Cast<string>();
                        default:
                            throw new Exception("Parameter type not supported");
                    }
                default:
                    throw new Exception("Parameter type not supported");
            }
        }
    }
}
