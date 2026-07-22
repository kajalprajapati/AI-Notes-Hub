using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    //<T> means Generic Type -It allows your class to work with any data type
    public class ApiResponse<T>
    {
        //public int StatusCode { get; set; }
        //public T Data { get; set; }
        //public int Count { get; set; }
        //public string Message { get; set; }
        //public bool Success { get; set; }

        [JsonPropertyOrder(1)]
        public bool Success { get; set; }

        [JsonPropertyOrder(2)]
        public int StatusCode { get; set; }

        [JsonPropertyOrder(3)]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public int? Count { get; set; }

        [JsonPropertyOrder(5)]
        public T? Data { get; set; }
        [JsonPropertyOrder(6)]

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }

        [JsonPropertyOrder(7)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

