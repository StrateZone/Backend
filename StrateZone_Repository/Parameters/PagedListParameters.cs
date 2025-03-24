using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Parameters
{
    public abstract class PagedListParameters
    {
        private const int maxPageSize = 50;

        private int pageSize = 10;

        [FromQuery(Name = "page-number")]
        public int PageNumber { get; set; } = 1;

        [FromQuery(Name = "page-size")]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value < 1 ? 10 : Math.Min(value, maxPageSize);
        }

        [FromQuery(Name = "order-by")]
        public string OrderBy { get; set; } = "id";
    }
}
