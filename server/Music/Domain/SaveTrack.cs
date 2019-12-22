﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel;
using Music.Domain.Shared;

namespace Music.Domain
{
    public class TrackUserProps
    {
        public long TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class SaveTrackYoutubeExecutor : ServiceResolverAware
    {
        public SaveTrackYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute(TrackUserProps trackProps)
        {

        }
    }
}
