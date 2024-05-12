using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Constant;
using MeWhen.Domain.DTO;
using MeWhen.Domain.Model;
using MeWhen.Domain.Validator;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using MeWhen.Service.App.Tag;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MeWhen.Service.App.Image
{
    [DataContract]
    public class InsertImageCommand : IRequest<InsertImageCommandResponse>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Source { get; set; }
        public required IFormFile Image { get; set; }
        public required List<string> Tags { get; set; } = [];
        public required ModelConstant.AgeRating AgeRating { get; set; }
        public required bool ConsentCompressed { get; set; }
    }

    public class InsertImageCommandResponse(Guid id)
    {
        public Guid ID { get; set; } = id;
    }

    public class InsertImageCommandValidator : AbstractValidator<InsertImageCommand>
    {
        public InsertImageCommandValidator()
        {
            RuleFor(x => x.Tags).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Image).NotNull().SetValidator(new IFormFileValidator());
        }
    }

    public class InsertImageCommandHandler(MeWhenDBContext _DB) : IRequestHandler<InsertImageCommand, InsertImageCommandResponse>
    {
        public async Task<InsertImageCommandResponse> Handle(InsertImageCommand request, CancellationToken cancellationToken)
        {
            // Masuk gambar
            var imageID = Guid.NewGuid();

            // Nyoba dulu apa gambarnya bisa dicompress jadi 20KB
            // Kl nga bisa, y sudahlah 😔
            var processedImage = new CompressedImageDTO(request.Image);

            var image = new ImageModel()
            {
                Name = request.Name,
                UploadDate = DateTime.Now.SpecifyKind(),
                AgeRating = request.AgeRating,
                ID = imageID,
                Extension = processedImage.Extension,
                Description = request.Description,
                Link = $"{imageID}.{processedImage.Extension}",
                Source = request.Source ?? ""
            };

            // Masuk tag yang not exists
            var tagInDB = _DB.Set<TagModel>()
                .Where(x => request.Tags.Contains(x.Name))
                .ToList();

            var tagNotInDB = request.Tags.Except(tagInDB.Select(x => x.Name))
                .Select(x => new TagModel()
                {
                    ID = Guid.NewGuid(),
                    Name = x,
                    AgeRating = ModelConstant.AgeRating.GENERAL
                })
                .ToList();

            var imageTag = tagInDB.Union(tagNotInDB)
                .Select(x => new ImageTagModel()
                {
                    ID = Guid.NewGuid(),
                    ImageID = imageID,
                    TagID = x.ID
                });

            await _DB.Transaction(async t =>
            {
                await _DB.AddAsync(image, cancellationToken);
                await _DB.AddRangeAsync(tagNotInDB, cancellationToken);
                await _DB.AddRangeAsync(imageTag, cancellationToken);

                await _DB.SaveChangesAsync(cancellationToken);
                var a = await FileHelper.UploadImage($"{imageID}.{processedImage.Extension}", processedImage.Content, cancellationToken);
                if (!a) throw new Exception("Upload file failed");
            });

            return new InsertImageCommandResponse(imageID);
        }
    }
}