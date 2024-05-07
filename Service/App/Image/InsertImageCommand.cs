using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Domain.Validator;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using MeWhen.Service.App.Tag;
using MeWhen.Service.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MeWhen.Service.App.Image
{
    public class InsertImageCommand : IRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Source { get; set; }
        public required IFormFile Image { get; set; }
        public required List<string> Tags { get; set; } = [];
        
        [BindNever]
        public string Extension => Image.FileName.Split(".", 2)[1];
    }

    public class InsertImageValidator : AbstractValidator<InsertImageCommand>
    {
        public InsertImageValidator()
        {
            RuleFor(x => x.Tags).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Image).NotNull().SetValidator(new IFormFileValidator());
        }
    }

    public class InsertImageCommandHandler(MeWhenDBContext _DB, IMediator _Mediator) : IRequestHandler<InsertImageCommand>
    {
        public async Task Handle(InsertImageCommand request, CancellationToken cancellationToken)
        {
            // Masuk gambar
            var imageID = Guid.NewGuid();            
            var image = new ImageModel(){
                Name = request.Name,
                UploadDate = DateTime.Now.SpecifyKind(),
                AgeRating = Domain.Constant.ModelConstant.AgeRating.GENERAL,
                ID = imageID,
                Extension = request.Extension,
                Description = request.Description,
                Link = $"{imageID}.{request.Extension}",
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
                    AgeRating = Domain.Constant.ModelConstant.AgeRating.GENERAL
                });

            var imageTag = tagInDB.Union(tagNotInDB)
                .Select(x => new ImageTagModel()
                {
                    ID = Guid.NewGuid(),
                    ImageID = imageID,
                    TagID = x.ID
                });
            
            await _DB.AddAsync(image, cancellationToken);
            await _DB.AddRangeAsync(tagNotInDB, cancellationToken);
            await _DB.AddRangeAsync(imageTag, cancellationToken);

            await _DB.SaveChangesAsync(cancellationToken);   
            await _Mediator.Send(new UploadImageCommand { File = request.Image, FileName = $"{imageID}.{request.Extension}" }, cancellationToken);
        }
    }
}