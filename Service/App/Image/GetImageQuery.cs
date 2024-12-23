using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Configuration;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeWhenAPI.Service.App.Image
{
    public class GetImageQuery : PaginationRequest<PaginationResponse<List<GetImageQueryResponse>>>
    {
        public List<string> TagAND { get; set; } = [];
        public List<string> TagOR { get; set; } = [];
        public ModelConstant.AgeRating AgeRating { get; set; } = ModelConstant.AgeRating.GENERAL;

        [BindNever]
        public List<string> JoinedTag => TagAND.Union(TagOR).ToList();
    }

    public class GetImageQueryValidator : PaginationRequestValidator<GetImageQuery> { }

    public class GetImageQueryResponse
    {
        public required Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
        public required string Source { get; set; }
        public required string Description { get; set; }
        public required DateTime UploadDate { get; set; }
        public required ModelConstant.AgeRating AgeRating { get; set; }
        public required List<string> Tags { get; set; }
    }

    public class GetImageQueryHandler(MeWhenDBContext _DB, IOptions<StorageConfiguration> _StorageConf, Supabase.Client _Supabase) : IRequestHandler<GetImageQuery, PaginationResponse<List<GetImageQueryResponse>>>
    {
        public async Task<PaginationResponse<List<GetImageQueryResponse>>> Handle(GetImageQuery request, CancellationToken cancellationToken)
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
                    ID = image.ID,
                    Name = image.Name,
                    AgeRating = image.AgeRating,
                    Source = image.Source,
                    Image = $"{link}/{image.ID}.{image.Extension}",
                    Tags = image.Tags.Select(x => x.Tag.Name).ToList(),
                    UploadDate = image.DateIn,
                    Description = image.Description
                }
            ).ToPaginationResult(request, cancellationToken);
        }
    }
}