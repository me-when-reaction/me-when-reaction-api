using System;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Service.App.Tag
{
    public class GetTagSuggestionQuery : IRequest<List<GetTagSuggestionResponse>>
    {
        /// <summary>
        /// Query yang paling terakhir
        /// </summary>
        /// <value></value>
        public string Query { get; set; } = "";

        /// <summary>
        /// Query yang belakang2 yang udah diketikkin
        /// </summary>
        /// <value></value>
        public List<string> ExistingQuery { get; set; } = [];
    }

    public class GetTagSuggestionQueryValidator : AbstractValidator<GetTagSuggestionQuery>
    {
        public GetTagSuggestionQueryValidator()
        {
        }
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
                    tag.Name.Contains(request.Query) &&
                    !request.ExistingQuery.Contains(tag.Name)
                select new GetTagSuggestionResponse()
                {
                    Name = tag.Name,
                    Count = tag.TagsUsed.Count()
                }
            )
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Name.Length)
            .ThenBy(x => x.Name)
            .Take(7)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
