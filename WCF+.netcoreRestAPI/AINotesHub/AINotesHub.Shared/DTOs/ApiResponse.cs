using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    //<T> means Generic Type -It allows your class to work with any data type
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
