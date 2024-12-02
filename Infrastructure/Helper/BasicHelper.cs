using System;
using MeWhenAPI.Domain.Configuration;
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

        public async static Task<List<T>> Paginate<T, U>(this IQueryable<T> query, U request, CancellationToken cancellationToken)
            where U : PaginationRequestPlain
        {
            if (request.PageSize == 0) return await query.ToListAsync(cancellationToken);
            else return await query.Skip(request.PageSize * (request.CurrentPage - 1)).Take(request.PageSize).ToListAsync(cancellationToken);
        }

        public async static Task<PaginationResponse<List<T>>> ToPaginationResult<T, U>(this IQueryable<T> query, U request, CancellationToken cancellationToken)
            where U : PaginationRequestPlain
        {
            return new()
            {
                PageSize = request.PageSize,
                CurrentPage = request.CurrentPage,
                TotalPage = ((await query.CountAsync(cancellationToken)) + request.PageSize - 1) / request.PageSize,
                Data = await query.Paginate(request, cancellationToken)
            };
        }
    }
}
