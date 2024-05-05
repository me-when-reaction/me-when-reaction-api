using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.Image
{
    public class GetImageQuery : IRequest<List<GetImageQueryResponse>>
    {
        public List<string> TagAND { get; set; } = [];
        public List<string> TagOR { get; set; } = [];

        [BindNever]
        public List<string> JoinedTag => TagAND.Union(TagOR).ToList();
    }

    public class GetImageQueryResponse
    {
        public required string Name { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageQueryHandler(MeWhenDBContext _DB) : IRequestHandler<GetImageQuery, List<GetImageQueryResponse>>
    {
        public async Task<List<GetImageQueryResponse>> Handle(GetImageQuery request, CancellationToken cancellationToken)
            => await (
                from i in _DB.Set<ImageModel>()
                    .Include(x => x.Tags)
                    .ThenInclude(y => y.Tag)
                let tags = i.Tags.Select(x => x.Tag.Name)
                where 
                    request.JoinedTag.Count == 0 ||
                    (
                        (request.TagAND.Count == 0 || request.TagAND.All(x => tags.Any(y => y == x))) &&
                        (request.TagOR.Count == 0 ||request.TagOR.Any(x => tags.Any(y => y == x)))
                    )
                select new GetImageQueryResponse()
                {
                    Name = i.Name,
                    Tags = i.Tags.Select(x => x.Tag.Name).ToList()
                }
            ).ToListAsync(cancellationToken: cancellationToken);
    }
}