using System;
using ClosedXML.Excel;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace MeWhen.Import
{
    public static class ImportScript
    {
        public class TempData
        {
            public Guid ID { get; set; }
            public required string Name { get; set; }
            public required string Tags { get; set; }
            public List<string> TagsList => [..Tags.Split(" ")];
        }

        public static void Import()
        {
            var worksheet = new XLWorkbook("./me-when.xlsx").Worksheet(1);
            List<TempData> data = [];
            var ctx = new MeWhenDBContext(new DbContextOptionsBuilder<MeWhenDBContext>()
                .UseNpgsql(Program.Config.GetConnectionString("Default")).Options);

            var rowIndex = 2;
            while(!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                var d = new TempData()
                {
                    ID = new(row.Cell(1).CachedValue.GetText()),
                    Name = row.Cell(2).CachedValue.GetText(),
                    Tags = row.Cell(6).CachedValue.GetText()
                };
                data.Add(d);
            }

            var a = string.Join("\n", data.SelectMany(x => x.TagsList)
                .Distinct());

            var tagsList = data.SelectMany(x => x.TagsList)
                .Distinct()
                .ToDictionary(k => k, v => Guid.NewGuid());

            // ctx.Set<ImageModel>().AddRange(data.Select(d => new Image()
            // {
            //     ID = d.ID,
            //     Name = d.Name,
            //     Tags = [.. d.Tags.Split(" ")],
            //     TagString = d.Tags
            // }));

            // ctx.Set<TagModel>().AddRange(tagsList.Select(x => new TagModel(){
            //     AgeRating = ModelConstant.AgeRating.GENERAL,
            //     Name = x.Key,
            //     ID = x.Value
            // }));

            // ctx.SaveChanges();
        }
    }
}
