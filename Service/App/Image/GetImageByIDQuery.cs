using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Configuration;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Exception;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeWhenAPI.Service.App.Image
{
    public class GetImageByIDQuery : IRequest<GetImageByIDQueryResponse>
    {
        public required Guid ID { get; set; }
    }

    public class GetImageByIDQueryValidator : AbstractValidator<GetImageByIDQuery>
    {
        public GetImageByIDQueryValidator()
        {
            RuleFor(x => x.ID).NotEmpty();
        }
    }

    public class GetImageByIDQueryResponse
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
        public required string Source { get; set; }
        public required string Description { get; set; }
        public required DateTime UploadDate { get; set; }
        public required ModelConstant.AgeRating AgeRating { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageByIDQueryHandler(MeWhenDBContext _DB, IOptions<StorageConfiguration> _StorageConf, Supabase.Client _Supabase) : IRequestHandler<GetImageByIDQuery, GetImageByIDQueryResponse>
    {
        public async Task<GetImageByIDQueryResponse> Handle(GetImageByIDQuery request, CancellationToken cancellationToken)
        {
            var link = (_StorageConf.Value.StorageType == FileConstant.StorageType.Native) ?
                _StorageConf.Value.AccessPath :
                    _Supabase.Storage
                    .From(_StorageConf.Value.Bucket)
                    .GetPublicUrl("")[..^1];

            return await (
                from image in _DB.Set<ImageModel>()
                    .Include(x => x.Tags)
                    .ThenInclude(y => y.Tag)
                let tags = image.Tags.Select(x => x.Tag.Name)
                where
                    image.ID == request.ID
                orderby image.DateIn descending
                select new GetImageByIDQueryResponse()
                {
                    Name = image.Name,
                    AgeRating = image.AgeRating,
                    Source = image.Source,
                    Image = $"{link}/{image.ID}.{image.Extension}",
                    Tags = image.Tags.Select(x => x.Tag.Name).ToList(),
                    UploadDate = image.DateIn,
                    Description = image.Description
                }
            ).FirstOrDefaultAsync(cancellationToken: cancellationToken) ?? throw new BadRequestException("ID not found");
        }
    }
}