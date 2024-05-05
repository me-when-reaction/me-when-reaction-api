using System;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.Tag
{
    public class GetTagSuggestionQuery : IRequest<List<GetTagSuggestionResponse>>
    {
        public string Query { get; set; } = "";
    }

    public class GetTagSuggestionResponse
    {
        public required string Name { get; set; }
        public required int Count { get; set; }
        public string NameCount => $"{Name} ({Count})";
    }

    public class GetTagSuggestionRequestHandler(MeWhenDBContext _DB) : IRequestHandler<GetTagSuggestionQuery, List<GetTagSuggestionResponse>>
    {
        public async Task<List<GetTagSuggestionResponse>> Handle(GetTagSuggestionQuery request, CancellationToken cancellationToken)
            => await (
                from tag in _DB.Set<TagModel>()
                    .Include(x => x.TagsUsed)
                where
                    !string.IsNullOrWhiteSpace(request.Query) &&
                    tag.Name.Contains(request.Query)
                select new GetTagSuggestionResponse()
                {
                    Name = tag.Name,
                    Count = tag.TagsUsed.Count()
                }
            )
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
