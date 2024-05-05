using System;

namespace MeWhen.Infrastructure.Helper
{
    public static class BasicHelper
    {
        public static Guid ToGUID(this string str) => new(str);
        public static DateTime SpecifyKind(this DateTime dt, DateTimeKind kind = DateTimeKind.Utc) => DateTime.SpecifyKind(dt, kind);
    }
}
