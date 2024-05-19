using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Configuration;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using MeWhen.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeWhen.Service.App.Image
{
    public class GetImageQuery : IRequest<List<GetImageQueryResponse>>
    {
        public List<string> TagAND { get; set; } = [];
        public List<string> TagOR { get; set; } = [];
        public ModelConstant.AgeRating AgeRating { get; set; } = ModelConstant.AgeRating.GENERAL;

        [BindNever]
        public List<string> JoinedTag => TagAND.Union(TagOR).ToList();

        /// <summary>
        /// No pagination = 0
        /// </summary>
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

    }

    public class GetImageQueryValidator : AbstractValidator<GetImageQuery>
    {
        public GetImageQueryValidator(IAuthUtilities _Auth)
        {
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(10)
                .When(x => !_Auth.IsAuthenticated())
                .WithMessage("No pagination is available only for authenticated users.");
            
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);
        }
    }

    public class GetImageQueryResponse
    {
        public required string Name { get; set; }
        public required string Link { get; set; }
        public required DateTime UploadDate { get; set; }
        public required ModelConstant.AgeRating AgeRating { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageQueryHandler(MeWhenDBContext _DB, IOptions<StorageConfiguration> _StorageConf, Supabase.Client _Supabase) : IRequestHandler<GetImageQuery, List<GetImageQueryResponse>>
    {
        public async Task<List<GetImageQueryResponse>> Handle(GetImageQuery request, CancellationToken cancellationToken)
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
                    (request.JoinedTag.Count == 0 ||
                    (
                        (request.TagAND.Count == 0 || request.TagAND.All(x => tags.Any(y => y == x))) &&
                        (request.TagOR.Count == 0 || request.TagOR.Any(x => tags.Any(y => y == x)))
                    )) && image.AgeRating <= request.AgeRating
                orderby image.DateIn descending
                select new GetImageQueryResponse()
                {
                    Name = image.Name,
                    AgeRating = image.AgeRating,
                    Link = $"{link}/{image.ID}.{image.Extension}",
                    Tags = image.Tags.Select(x => x.Tag.Name).ToList(),
                    UploadDate = image.DateIn
                }
            ).Paginate(request.PageSize, request.PageNumber, cancellationToken);
        }
    }
}