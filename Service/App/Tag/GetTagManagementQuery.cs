using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Configuration;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using MeWhenAPI.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Service.App.Tag
{
    public class GetTagManagementQuery : PaginationRequest<PaginationResponse<List<GetTagManagementQueryResponse>>>
    {
        public string? Query { get; set; }
        public int MinUsage { get; set; } = -1;
        public int MaxUsage { get; set; } = -1;
    }

    public class GetTagManagementQueryResponse
    {
        public required Guid ID { get; set; }
        public required string Tag { get; set; }
        public required int Usage { get; set; }
        public List<string> Alias { get; set; } = [];
    }

    public class GetTagManagementQueryValidator : PaginationRequestValidator<GetTagManagementQuery>
    {
        public GetTagManagementQueryValidator()
        {
            RuleFor(x => x.MinUsage).GreaterThanOrEqualTo(-1);
            RuleFor(x => x.MaxUsage).GreaterThanOrEqualTo(-1);
        }
    }

    public class GetTagManagementQueryHandler(MeWhenDBContext _DB) : IRequestHandler<GetTagManagementQuery, PaginationResponse<List<GetTagManagementQueryResponse>>>
    {
        public async Task<PaginationResponse<List<GetTagManagementQueryResponse>>> Handle(GetTagManagementQuery request, CancellationToken cancellationToken)
        {
            return await _DB.Set<TagModel>()
                .IsNotDeletedAnd(x =>
                    (string.IsNullOrWhiteSpace(request.Query) || x.Name.Contains(request.Query)) &&
                    (request.MinUsage == -1 || x.TagsUsed.Count >= request.MinUsage) &&
                    (request.MaxUsage == -1 || x.TagsUsed.Count <= request.MaxUsage)
                )
                .Select(x => new GetTagManagementQueryResponse()
                {
                    ID = x.ID,
                    Tag = x.Name,
                    Usage = x.TagsUsed.Count
                })
                .ToPaginationResult(request, cancellationToken);
        }
    }
}