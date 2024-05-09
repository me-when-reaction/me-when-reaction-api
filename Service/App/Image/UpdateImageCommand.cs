using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Exception;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.App.Image
{
    public class UpdateImageCommand : IRequest
    {
        public required Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Source { get; set; }
        public List<string>? Tags { get; set; } = [];
        public ModelConstant.AgeRating? AgeRating { get; set; }
    }

    public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
    {
        public UpdateImageCommandValidator()
        {

        }
    }

    public class UpdateImageCommandHandler(MeWhenDBContext _DB) : IRequestHandler<UpdateImageCommand>
    {
        public async Task Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            var image = _DB
                .Set<ImageModel>()
                .Include(x => x.Tags)
                .ThenInclude(y => y.Tag)
                .FirstOrDefault(x => x.ID == request.ID)
            ?? throw new BadRequestException("Image not found");

            if (!string.IsNullOrWhiteSpace(request.Name)) image.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Description)) image.Description = request.Description;
            if (!string.IsNullOrWhiteSpace(request.Source)) image.Source = request.Source;
            if (request.AgeRating.HasValue) image.AgeRating = request.AgeRating.Value;


            // Hapus yang nga ada di list
            if (request.Tags != null && request.Tags.Count != 0)
            {
                // Tag ini bakal dihapus
                var deletedTag = image.Tags.Where(x => !request.Tags.Contains(x.Tag.Name)).ToList();
                
                // Sisa tag yang available
                var unchangedTag = image.Tags.Where(x => request.Tags.Contains(x.Tag.Name)).Select(x => x.Tag.Name);

                // Cari tag yang ada pada DB dan belum ada pada available
                var tagInDB = _DB.Set<TagModel>()
                    .Where(x => request.Tags.Contains(x.Name) && !unchangedTag.Contains(x.Name))
                    .ToList();

                var tagNotInDB = request.Tags.Where(x => !tagInDB.Select(y => y.Name).Contains(x) && !unchangedTag.Contains(x))
                    .Select(x => new TagModel()
                    {
                        ID = Guid.NewGuid(),
                        Name = x,
                        AgeRating = ModelConstant.AgeRating.GENERAL
                    })
                    .ToList();

                
                // Insert imagetag baru ini
                var imageTag = tagInDB.Union(tagNotInDB)
                .Select(x => new ImageTagModel()
                {
                    ID = Guid.NewGuid(),
                    ImageID = image.ID,
                    TagID = x.ID
                }).ToList();
            

                _DB.AddRange(tagNotInDB);
                if (deletedTag.Count > 0) _DB.RemoveRange(deletedTag);
                _DB.AddRange(imageTag);
            }

            _DB.Update(image);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}