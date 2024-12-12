using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.DTO;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Domain.Validator;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using MeWhenAPI.Infrastructure.Utilities;
using MeWhenAPI.Service.App.Tag;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MeWhenAPI.Service.App.Image
{
    public class InsertImageCommand : IRequest<InsertImageCommandResponse>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Source { get; set; }
        public required IFormFile Image { get; set; }
        public required List<string> Tags { get; set; } = [];
        public required ModelConstant.AgeRating AgeRating { get; set; }
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
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.AgeRating).NotNull();
            RuleFor(x => x.Image).NotNull().SetValidator(new FormFileValidator());
        }
    }

    public class InsertImageCommandHandler(MeWhenDBContext _DB, IAuthUtilities _Auth, IFileUtilities _File) : IRequestHandler<InsertImageCommand, InsertImageCommandResponse>
    {
        public async Task<InsertImageCommandResponse> Handle(InsertImageCommand request, CancellationToken cancellationToken)
        {
            // Masuk gambar
            var imageID = Guid.NewGuid();
            var userID = _Auth.GetUserID();

            // Nyoba dulu apa gambarnya bisa dicompress jadi 20KB
            // Kl nga bisa, y sudahlah 😔
            var processedImage = new CompressedImageDTO(request.Image);

            var image = new ImageModel()
            {
                Name = request.Name,
                AgeRating = request.AgeRating,
                ID = imageID,
                Extension = processedImage.Extension,
                Description = request.Description,
                Link = $"{imageID}.{processedImage.Extension}",
                Source = request.Source ?? "",
                UserIn = userID
            };

            // Proses tag yang ada dilisting di DB
            var remainingTag = request.Tags.Select(x => x.Replace(' ', '_')).ToList(); 
            var tagInDB = _DB.Set<TagModel>()
                .Where(x => remainingTag.Contains(x.Name))
                .ToList();

            // Proses tag yang ada di alias di DB. Ambil di DB tag mana yang alias ada di salah satu tag yang belum diproses
            // Hasilnya tag yang dikaitin dengan alias akan dianggap punya tag tsb
            remainingTag = remainingTag.Except(tagInDB.Select(x => x.Name)).ToList();
            var tagInAlias = _DB.Set<TagModel>()
                .Where(x => x.Alias.Intersect(remainingTag).Any())
                .ToList();

            // Proses tag yang tidak ada di DB dan tidak ada di alias
            remainingTag = remainingTag.Except(tagInAlias.Select(x => x.Name)).ToList();
            var tagNotInDB = remainingTag.Except(tagInDB.Select(x => x.Name))
                .Select(x => new TagModel()
                {
                    ID = Guid.NewGuid(),
                    Name = x,
                    AgeRating = ModelConstant.AgeRating.GENERAL,
                    Alias = [],
                    UserIn = userID
                })
                .ToList();

            var imageTag = tagInDB.Union(tagInAlias).Union(tagNotInDB)
                .Select(x => new ImageTagModel()
                {
                    ID = Guid.NewGuid(),
                    ImageID = imageID,
                    TagID = x.ID,
                    UserIn = userID
                });

            await _DB.Transaction(async t =>
            {
                await _DB.AddAsync(image, cancellationToken);
                await _DB.AddRangeAsync(tagNotInDB, cancellationToken);
                await _DB.AddRangeAsync(imageTag, cancellationToken);

                await _DB.SaveChangesAsync(cancellationToken);
                await _File.UploadImage($"{imageID}.{processedImage.Extension}", processedImage.Content, cancellationToken);
            });

            return new InsertImageCommandResponse(imageID);
        }
    }
}