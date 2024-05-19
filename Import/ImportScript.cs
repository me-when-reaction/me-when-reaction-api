using System;
using ClosedXML.Excel;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using static MeWhenAPI.Domain.Constant.ModelConstant;

namespace MeWhenAPI.Import
{
    public static class ImportScript
    {
        public static void Import()
        {
            // reset data

            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .Build();

            var workBook = new XLWorkbook("./me-when.xlsx");
            var ctx = new MeWhenDBContext(new DbContextOptionsBuilder<MeWhenDBContext>()
                .UseNpgsql(conf.GetConnectionString("Default")).Options);

            ctx.Set<ImageTagModel>().ExecuteDelete();
            ctx.Set<ImageModel>().ExecuteDelete();
            ctx.Set<TagModel>().ExecuteDelete();

            ctx.SaveChanges();

            List<ImageModel> image = [];
            List<ImageTagModel> imageTag = [];
            List<TagModel> tag = [];

            var rowIndex = 2;
            var worksheet = workBook.Worksheet(1);
            while (!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                image.Add(new ImageModel()
                {
                    ID = row.Cell(1).CachedValue.GetText().ToGUID(),
                    Name = row.Cell(2).CachedValue.GetText(),
                    Link = row.Cell(3).IsEmpty() ? "" : row.Cell(3).CachedValue.GetText(),
                    Extension = row.Cell(4).CachedValue.GetText(),
                    Description = row.Cell(5).CachedValue.GetText(),
                    Source = row.Cell(6).IsEmpty() ? "" : row.Cell(5).CachedValue.GetText(),
                    AgeRating = row.Cell(8).CachedValue.GetText() switch
                    {
                        "MATURE" => AgeRating.MATURE,
                        "EXPLICIT" => AgeRating.EXPLICIT,
                        _ => AgeRating.GENERAL,
                    },
                    UserIn = new Guid("4bb78760-fd94-415f-8eb3-bd86377f7a4d")
                });
            }

            rowIndex = 2;
            worksheet = workBook.Worksheet(3);
            while (!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                tag.Add(new TagModel()
                {
                    ID = row.Cell(1).CachedValue.GetText().ToGUID(),
                    Name = row.Cell(2).CachedValue.GetText(),
                    AgeRating = row.Cell(3).CachedValue.GetText() switch
                    {
                        "MATURE" => AgeRating.MATURE,
                        "EXPLICIT" => AgeRating.EXPLICIT,
                        _ => AgeRating.GENERAL,
                    },
                    UserIn = new Guid("4bb78760-fd94-415f-8eb3-bd86377f7a4d")
                });
            }

            rowIndex = 2;
            worksheet = workBook.Worksheet(2);
            while (!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                imageTag.Add(new ImageTagModel()
                {
                    ID = row.Cell(1).CachedValue.GetText().ToGUID(),
                    ImageID = row.Cell(2).CachedValue.GetText().ToGUID(),
                    TagID = row.Cell(4).CachedValue.GetText().ToGUID(),
                    UserIn = new Guid("4bb78760-fd94-415f-8eb3-bd86377f7a4d")
                });
            }

            ctx.Set<ImageModel>().AddRange(image);
            ctx.Set<TagModel>().AddRange(tag);
            ctx.Set<ImageTagModel>().AddRange(imageTag);

            ctx.SaveChanges();
        }
    }
}
