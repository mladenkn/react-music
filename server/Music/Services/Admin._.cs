using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Newtonsoft.Json;
using Utilities;

namespace Music.Services
{
    public partial class AdminService : ServiceResolverAware
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<object> ExecuteCsCommand(string code) => Resolve<CSharpCodeExecutor>().Execute(code);

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

                        var result = await CallMethod(className, methodName, params_);
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

        public async Task<AdminSectionParams> GetAdminSectionParams()
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var commands = await Query<CsCommand>()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .Select(c => new CsCommandForAdminSection
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code
                })
                .ToArrayAsync();

            var sectionStateJson = await Query<AdminUser>()
                .Where(u => u.Id == userId)
                .Select(u => u.AdminSectionStateJson)
                .FirstOrDefaultAsync();

            var currentCommandId = string.IsNullOrEmpty(sectionStateJson)
                ? (int?) null
                : JsonConvert.DeserializeObject<AdminSectionState>(sectionStateJson).CurrentCommandId;

            var r = new AdminSectionParams
            {
                Commands = commands,
                CurrentCommandId = currentCommandId
            };

            return r;
        }

        public async Task SaveSectionState(AdminSectionState state)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<AdminUser>().FirstOrDefaultAsync(u => u.Id == userId);
            user.AdminSectionStateJson = JsonConvert.SerializeObject(state);
            Db.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task<CsCommandForAdminSection> Add(CsCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var cmdDbEntity = new CsCommand
            {
                UserId = userId,
                Name = cmd.Name,
                Code = cmd.Code,
                AddedAt = DateTime.Now
            };
            Db.Add(cmdDbEntity);
            await Db.SaveChangesAsync();
            return new CsCommandForAdminSection
            {
                Id = cmdDbEntity.Id,
                Name = cmdDbEntity.Name,
                Code = cmdDbEntity.Code
            };
        }

        public async Task Update(CsCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            
            var cmdFromDb = await Query<CsCommand>()
                .FirstOrDefaultAsync(c => c.Id == cmd.Id);

            if (cmdFromDb == null)
                throw new ApplicationException("Command not found.");
            if(cmdFromDb.UserId != userId)
                throw new ApplicationException("Trying to update other user's command.");

            cmdFromDb.Name = cmd.Name;
            cmdFromDb.Code = cmd.Code;

            Db.Update(cmdFromDb);
            await Db.SaveChangesAsync();
        }

        public Task SetVariable(string key, object value)
        {
            var variableService = Resolve<PersistantVariablesService>();
            return variableService.Set(key, value);
        }
    }
}
