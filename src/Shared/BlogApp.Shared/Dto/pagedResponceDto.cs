using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Shared.Dto
{
    public class pagedResponceDto<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }

    }
}
