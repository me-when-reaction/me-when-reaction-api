using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Configuration;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeWhenAPI.Service.App.Image
{
    public class GetRandomImageQuery : IRequest<GetRandomImageQueryResponse?>
    {
        public ModelConstant.AgeRating AgeRating { get; set; } = ModelConstant.AgeRating.GENERAL;
    }

    public class GetRandomImageQueryValidator : AbstractValidator<GetRandomImageQuery>
    {
        public GetRandomImageQueryValidator()
        {
            RuleFor(x => x.AgeRating).IsInEnum();
        }
    }

    public class GetRandomImageQueryResponse
    {
        public required Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Link { get; set; }
        public required DateTime UploadDate { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetRandomImageQueryHandler(MeWhenDBContext _DB, IOptions<StorageConfiguration> _StorageConf, Supabase.Client _Supabase) : IRequestHandler<GetRandomImageQuery, GetRandomImageQueryResponse?>
    {
        public async Task<GetRandomImageQueryResponse?> Handle(GetRandomImageQuery request, CancellationToken cancellationToken)
        {
            var link = (_StorageConf.Value.StorageType == FileConstant.StorageType.Native) ?
                _StorageConf.Value.AccessPath :
                    _Supabase.Storage
                    .From(_StorageConf.Value.Bucket)
                    .GetPublicUrl("")[..^1];
            return await (
                from image in _DB.Set<ImageModel>().Include(x => x.Tags).ThenInclude(x => x.Tag)
                orderby image.DateIn descending
                select new GetRandomImageQueryResponse()
                {
                    ID = image.ID,
                    Name = image.Name,
                    Link = $"{link}/{image.ID}.{image.Extension}",
                    UploadDate = image.DateIn,
                    Tags = image.Tags.Select(x => x.Tag.Name).ToList()
                }
            )
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}