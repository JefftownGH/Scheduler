﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerAPI.Dtos
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 10;
    }
}
