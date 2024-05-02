using System;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MeWhen.Application.Operation.Image.Query
{
    public class GetImageTagQuery
    {
        public required string Query { get; set; }
    }

    public class GetImageTagQueryHandler(MeWhenDBContext db)
    {
        private readonly MeWhenDBContext _DB = db;

        public static List<string> Handle(GetImageTagQuery data)
        {
            // var a = _DB.Set<Domain.Model.Image>();

            return ["Hello", "World"];
        }
    }
}
