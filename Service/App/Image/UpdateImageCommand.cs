using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Exception;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using MeWhenAPI.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Service.App.Image
{
    public class UpdateImageCommand : IRequest
    {
        public required Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Source { get; set; }
        public required List<string> Tags { get; set; } = [];
        public required ModelConstant.AgeRating? AgeRating { get; set; }
    }

    public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
    {
        public UpdateImageCommandValidator()
        {
            RuleFor(x => x.ID).NotEmpty();
            RuleFor(x => x.Tags).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Source).NotEmpty();
            RuleFor(x => x.AgeRating).NotNull();
        }
    }

    public class UpdateImageCommandHandler(MeWhenDBContext _DB, IAuthUtilities _Auth) : IRequestHandler<UpdateImageCommand>
    {
        public async Task Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            var userID = _Auth.GetUserID();
            var image = _DB
                .Set<ImageModel>()
                .Include(x => x.Tags)
                .ThenInclude(y => y.Tag)
                .FirstOrDefault(x => x.ID == request.ID)
            ?? throw new BadRequestException("Image not found");

            bool changed = false;
            if (!string.IsNullOrWhiteSpace(request.Name)) { image.Name = request.Name; changed = true; }
            if (!string.IsNullOrWhiteSpace(request.Description)) { image.Description = request.Description; changed = true; }
            if (!string.IsNullOrWhiteSpace(request.Source)) { image.Source = request.Source; changed = true; }
            if (request.AgeRating.HasValue) { image.AgeRating = request.AgeRating.Value; changed = true; }
            if (changed)
            {
                image.UserUp = userID;
                image.DateUp = DateTime.UtcNow;
            }

            // Hapus yang nga ada di list
            if (request.Tags != null && request.Tags.Count != 0)
            {
                var newTagName = request.Tags.Select(x => x.Replace(' ', '_')).ToList();
                var oldTag = image.Tags;
                var oldTagName = image.Tags.Select(x => x.Tag.Name);

                // 1 Yang ada di old, gaada di new, maka hapus, artinya tag dicabut dari gambar
                var deletedTag = oldTag.Where(o => !newTagName.Contains(o.Tag.Name));
                var deletedTagName = deletedTag.Select(x => x.Tag.Name);

                // 2 Yang tag deleted udah dihandle, sekarang yang masih exist di old dan new kita exclude dulu ya
                var remainingTag = newTagName.Except(deletedTagName).Except(oldTagName).ToList();

                // 3 Tag yang hilang di newTag dan sama percis dengan yang di old sudah diexclude
                // Kasusnya di remainingTag HANYA ada tag yang baru
                // Kita mau cek apa ada tag yang baru ini yang ada di DB
                // tagInDB = tag new yang udah ada di DB
                var tagInDB = _DB.Set<TagModel>()
                    .Where(x => remainingTag.Contains(x.Name))
                    .ToList();

                // 4 Oke, sekarang mungkin ada tag yang nga ada di DB
                // Tapi bisa aja itu tag hanya sebuah alias
                // Kita cari tag yang ada di alias
                remainingTag = remainingTag.Except(tagInDB.Select(x => x.Name)).ToList();
                var tagInAlias = _DB.Set<TagModel>()
                    .Where(x => x.Alias.Intersect(remainingTag).Any())
                    .ToList();

                // 5 Jika sampai sini remainingTag masih ada, maka berarti ini tag benar2 baru
                // Bikin tag baru
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

                // Insert tag yang new2
                var imageTag = tagInDB.Union(tagInAlias).Union(tagNotInDB)
                    .Select(x => new ImageTagModel()
                    {
                        ID = Guid.NewGuid(),
                        ImageID = image.ID,
                        TagID = x.ID,
                        UserIn = userID
                    });

                if (deletedTag.Any()) _DB.RemoveRange(deletedTag);
                _DB.AddRange(tagNotInDB);
                _DB.AddRange(imageTag);
            }

            _DB.Update(image);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}