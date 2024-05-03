using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.Image
{
    public class GetImageQuery : IRequest<List<GetImageQueryResponse>>
    {
        public List<string> Tag { get; set; } = [];
    }

    public class GetImageQueryResponse
    {
        public required string Name { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageQueryHandler(MeWhenDBContext _DB) : IRequestHandler<GetImageQuery, List<GetImageQueryResponse>>
    {
        public async Task<List<GetImageQueryResponse>> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            return (await _DB.Set<ImageModel>()
                .ToListAsync(cancellationToken: cancellationToken))
                .Select(x => new GetImageQueryResponse()
                {
                    Name = x.Name,
                    Tags = []
                }).ToList();
        }
    }
}