using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.Shared.Entities;

namespace AINotesHub.Shared.DTOs


{
    public class PagedResponse<T>
    {
        /// <summary>
        /// ///////////
        /// </summary>
        ////

        public bool Success { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public List<T> Items { get; set; } = new();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

    }
}
