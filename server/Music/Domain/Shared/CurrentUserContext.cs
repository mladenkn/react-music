﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.DataAccess.Models;

namespace Music.Domain.Shared
{
    public interface ICurrentUserContext
    {
        int Id { get; }
    }
}
