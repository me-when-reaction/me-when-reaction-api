using System;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Infrastructure.Helper
{
    public static class BasicHelper
    {
        public static Guid ToGUID(this string str) => new(str);
        public static DateTime SpecifyKind(this DateTime dt, DateTimeKind kind = DateTimeKind.Utc) => DateTime.SpecifyKind(dt, kind);

        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream is MemoryStream ms) return ms.ToArray();
            using var mems = new MemoryStream();
            stream.CopyTo(mems);
            return mems.ToArray();
        }

        public async static Task<List<T>> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            if (pageSize == 0) return await query.ToListAsync(cancellationToken);
            else return await query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync(cancellationToken);
        }
    }
}
