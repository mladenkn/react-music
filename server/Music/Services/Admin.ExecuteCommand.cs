﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utilities;

namespace Music.Services
{
    public partial class AdminService
    {
        public async Task<object> ExecuteCommand(IReadOnlyDictionary<string, object> cmd)
        {
            var type = cmd.Get<string>("type");
            
            async Task<object> Execute()
            {
                switch (type)
                {
                    case "AddTracksToYouTubeVideos":
                    {
                        var videoIds = cmd.Get<IEnumerable<string>>("videoIds");
                        return await Resolve<YouTubeVideosService>().AddTracksToVideos(videoIds);
                    }
                    case "DeleteTracks":
                    {
                        var trackIds = cmd.Get<IEnumerable<long>>("tracks").ToArray();
                        await Resolve<TracksService>().Delete(trackIds);
                        return "Successfully deleted all stated tracks";
                    }
                    case "CallMethod":
                    {
                        var methodParam = cmd.Get<string>("method");
                        var methodParamIndexOfDot = methodParam.IndexOf(".")!;

                        var className = methodParam.Substring(0, methodParamIndexOfDot);
                        var methodName = methodParam.Substring(methodParamIndexOfDot + 1);
                        var params_ = cmd.GetOrDefault<IReadOnlyDictionary<string, object>>("params") ?? new Dictionary<string, object>();

                        var classTypeInfo = GetClass(className);
                        var matchingMethods = GetMatchingMethods(classTypeInfo, methodName, params_).ToArray();

                        if (matchingMethods.Length == 0)
                            throw new ApplicationException("No matching methods found");
                        if (matchingMethods.Length != 1)
                            throw new ApplicationException($"Found {matchingMethods.Length} matching methods");

                        var classInstance = _serviceProvider.GetService(classTypeInfo);
                        var method = matchingMethods.Single();
                        var paramsAdjusted = AdjustParams(params_, method.GetParameters());
                        var result = await CallMethod(classInstance, method, paramsAdjusted);

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
                var r = await Execute();
                return r;
            }
        }

        private TypeInfo GetClass(string name)
        {
            var assembly = typeof(AdminService).Assembly;
            var classTypeInfo = assembly.DefinedTypes.Single(t => t.Name == name);
            return classTypeInfo;
        }

        private IEnumerable<MethodInfo> GetMatchingMethods(TypeInfo classTypeInfo, string methodName, IReadOnlyDictionary<string, object> params_)
        {
            return classTypeInfo.DeclaredMethods.Where(m =>
            {
                var parametersInfo = m.GetParameters();
                return m.Name == methodName && 
                       parametersInfo.Length == params_.Count() &&
                       DoParamsMatch(params_, parametersInfo);
            });
        }

        private bool DoParamsMatch(IReadOnlyDictionary<string, object> params_, IReadOnlyList<ParameterInfo> methodParams)
        {
            var index = 0;
            foreach (var param in params_)
            {
                var paramInfo = methodParams[index];
                if (paramInfo.Name != param.Key)
                    return false;
                index++;
            }

            return true;
        }

        private IReadOnlyDictionary<string, object> AdjustParams(IReadOnlyDictionary<string, object> params_, IReadOnlyList<ParameterInfo> methodParams)
        {
            var nonMatchingParams = GetNonMatchingParams(params_, methodParams);
            var newParamsPairs = params_.Select(param =>
            {
                var nonMatchingParam = nonMatchingParams.FirstOrDefault(nonMatchingParam => nonMatchingParam.key == param.Key);
                if (nonMatchingParam == default)
                    return param;
                else if (nonMatchingParam.expectedType == typeof(int) && nonMatchingParam.actualType == typeof(long))
                    return new KeyValuePair<string, object>(param.Key, (int)(long)param.Value);
                else if (nonMatchingParam.expectedType == typeof(long) && nonMatchingParam.actualType == typeof(int))
                    return new KeyValuePair<string, object>(param.Key, (long)(int)param.Value);
                else if (nonMatchingParam.expectedType == typeof(int?) && nonMatchingParam.actualType == typeof(long))
                    return new KeyValuePair<string, object>(param.Key, (int)(long)param.Value);
                else if (nonMatchingParam.expectedType == typeof(long?) && nonMatchingParam.actualType == typeof(int))
                    return new KeyValuePair<string, object>(param.Key, (long)(int)param.Value);
                else
                    throw new NotSupportedException();
            });
            return new Dictionary<string, object>(newParamsPairs);
        }

        private IEnumerable<(string key, Type expectedType, Type actualType)> GetNonMatchingParams(IReadOnlyDictionary<string, object> params_, IReadOnlyList<ParameterInfo> methodParams)
        {
            var index = 0;
            foreach (var param in params_)
            {
                var paramInfo = methodParams[index];
                if (!paramInfo.ParameterType.IsInstanceOfType(param.Value))
                    yield return (param.Key, paramInfo.ParameterType, param.Value.GetType());
                index++;
            }
        }

        private async Task<object> CallMethod(object classInstance, MethodInfo method, IReadOnlyDictionary<string, object> params_)
        {
            var paramsTransformed = params_.Select(e => e.Value).ToArray();
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
    }
}
