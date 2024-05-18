using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhen.Domain.Configuration
{
    public class StorageConfiguration
    {
        public required string ID { get; set; }
        public required string Secret { get; set; }
        public required string Endpoint { get; set; }
        public required string Bucket { get; set; }
        public required string NativePath { get; set; }
        public required string StorageType { get; set; }
    }
}