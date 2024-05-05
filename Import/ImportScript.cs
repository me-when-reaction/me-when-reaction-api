using System;
using ClosedXML.Excel;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using static MeWhen.Domain.Constant.ModelConstant;

namespace MeWhen.Import
{
    public static class ImportScript
    {
        public static void Import()
        {
            var workBook = new XLWorkbook("./me-when.xlsx");
            var ctx = new MeWhenDBContext(new DbContextOptionsBuilder<MeWhenDBContext>()
                .UseNpgsql(Program.Config.GetConnectionString("Default")).Options);

            List<ImageModel> image = [];
            List<ImageTagModel> imageTag = [];
            List<TagModel> tag = [];

            var rowIndex = 2;
            var worksheet = workBook.Worksheet(1);
            while(!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                image.Add(new ImageModel()
                {
                    ID = row.Cell(1).CachedValue.GetText().ToGUID(),
                    Name = row.Cell(2).CachedValue.GetText(),
                    Link = row.Cell(3).IsEmpty() ? "" : row.Cell(3).CachedValue.GetText(),
                    Description = row.Cell(4).CachedValue.GetText(),
                    Source = row.Cell(5).IsEmpty() ? "" : row.Cell(5).CachedValue.GetText(),
                    AgeRating = row.Cell(7).CachedValue.GetText() switch
                    {
                        "MATURE" => AgeRating.MATURE,
                        "EXPLICIT" => AgeRating.EXPLICIT,
                        _ => AgeRating.GENERAL,
                    },
                    UploadDate = DateTime.Now.SpecifyKind(),
                });
            }

            rowIndex = 2;
            worksheet = workBook.Worksheet(3);
            while(!worksheet.Cell(rowIndex, 1).IsEmpty())
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
                    }
                });
            }

            rowIndex = 2;
            worksheet = workBook.Worksheet(2);
            while(!worksheet.Cell(rowIndex, 1).IsEmpty())
            {
                var row = worksheet.Row(rowIndex++);
                imageTag.Add(new ImageTagModel()
                {
                    ID = row.Cell(1).CachedValue.GetText().ToGUID(),
                    ImageID = row.Cell(2).CachedValue.GetText().ToGUID(),
                    TagID = row.Cell(4).CachedValue.GetText().ToGUID()
                });
            }

            ctx.Set<ImageModel>().AddRange(image);
            ctx.Set<TagModel>().AddRange(tag);
            ctx.Set<ImageTagModel>().AddRange(imageTag);

            ctx.SaveChanges();
        }
    }
}
