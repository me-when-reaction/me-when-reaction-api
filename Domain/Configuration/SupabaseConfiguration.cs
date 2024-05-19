using System;

namespace MeWhenAPI.Domain.Configuration
{
    public class SupabaseConfiguration
    {
        public required string URL { get; set; }
        public required string Key { get; set; }
    }
}
