using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhenAPI.Domain.Configuration
{
    public class PaginationResponse<TData>
    {
        public required int TotalPage { get; set; }
        public required int PageSize { get; set; }
        public required int CurrentPage { get; set; }
        public bool IsLast => TotalPage == CurrentPage;
        public required TData Data { get; set; }
    }
}