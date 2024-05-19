using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhenAPI.Domain.DTO
{
    public class BaseMessageDTO
    {
        public DateTime MessageDate { get; set; } = DateTime.UtcNow;
        public required int StatusCode { get; set; }
        public required string Message { get; set; }
        public required object Data { get; set; }
    }
}