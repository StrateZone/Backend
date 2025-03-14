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
        const int maxPageSize = 50;

        [FromQuery(Name = "page-number")]
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        [FromQuery(Name = "page-size")]
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        [FromQuery(Name = "order-by")]
        public string OrderBy { get; set; } = "id";
    }
}
