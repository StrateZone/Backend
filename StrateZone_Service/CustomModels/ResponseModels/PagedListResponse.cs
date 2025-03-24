using MealHunt_Repositories.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class PagedListResponse<T>
    {
        public PagedList<T> PagedList { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevious { get; set; }

        public bool HasNext { get; set; }

        public PagedListResponse(PagedList<T> pagedList)
        {
            PagedList = pagedList;
            CurrentPage = pagedList.CurrentPage;
            TotalPages = pagedList.TotalPages;
            PageSize = pagedList.PageSize;
            TotalCount = pagedList.TotalCount;
            HasPrevious = pagedList.HasPrevious;
            HasNext = pagedList.HasNext;
        }
    }
}
