using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.App.Image
{
    public class GetImageQuery : IRequest<List<GetImageQueryResponse>>
    {
        public List<string> TagAND { get; set; } = [];
        public List<string> TagOR { get; set; } = [];
        public ModelConstant.AgeRating AgeRating { get; set; } = ModelConstant.AgeRating.GENERAL;

        [BindNever]
        public List<string> JoinedTag => TagAND.Union(TagOR).ToList();
    }

    public class GetImageQueryResponse
    {
        public required string Name { get; set; }
        public required string Link { get; set; }
        public required DateTime UploadDate { get; set; }
        public required ModelConstant.AgeRating AgeRating { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageQueryHandler(MeWhenDBContext _DB, IConfiguration _Config) : IRequestHandler<GetImageQuery, List<GetImageQueryResponse>>
    {
        public async Task<List<GetImageQueryResponse>> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            var link = _Config.GetValue<string>("TempPath");

            return await (
                from i in _DB.Set<ImageModel>()
                    .Include(x => x.Tags)
                    .ThenInclude(y => y.Tag)
                let tags = i.Tags.Select(x => x.Tag.Name)
                where
                    (request.JoinedTag.Count == 0 ||
                    (
                        (request.TagAND.Count == 0 || request.TagAND.All(x => tags.Any(y => y == x))) &&
                        (request.TagOR.Count == 0 || request.TagOR.Any(x => tags.Any(y => y == x)))
                    )) && i.AgeRating <= request.AgeRating
                orderby i.UploadDate descending
                select new GetImageQueryResponse()
                {
                    Name = i.Name,
                    AgeRating = i.AgeRating,
                    Link = $"{link}/{i.ID}.{i.Extension}",
                    Tags = i.Tags.Select(x => x.Tag.Name).ToList(),
                    UploadDate = i.UploadDate
                }
            ).Take(10).ToListAsync(cancellationToken: cancellationToken);
        }
    }
}