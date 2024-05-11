using System;

namespace MeWhen.Infrastructure.Helper
{
    public static class BasicHelper
    {
        public static Guid ToGUID(this string str) => new(str);
        public static DateTime SpecifyKind(this DateTime dt, DateTimeKind kind = DateTimeKind.Utc) => DateTime.SpecifyKind(dt, kind);

        public static byte[] ToByteArray(this Stream stream) {
            if (stream is MemoryStream ms) return ms.ToArray();
            using var mems = new MemoryStream();
            stream.CopyTo(mems);
            return mems.ToArray();
        }
    }
}
