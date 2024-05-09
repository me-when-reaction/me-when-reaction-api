using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhen.Domain.DTO
{
    public class BaseMessageDTO
    {
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public required int StatusCode { get; set; }
        public required object Data { get; set; }
    }
}