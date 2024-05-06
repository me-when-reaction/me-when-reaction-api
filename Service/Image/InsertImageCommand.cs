using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using MeWhen.Service.Tag;

namespace MeWhen.Service.Image
{
    public class InsertImageCommand : IRequest
    {
        public required string Name { get; set; }
        public required List<string> Tags { get; set; } = [];
    }

    public class InsertImageValidator : AbstractValidator<InsertImageCommand>
    {
        public InsertImageValidator()
        {
            RuleFor(x => x.Tags).NotEmpty();
        }
    }

    public class InsertImageCommandHandler(MeWhenDBContext _DB, IMediator _Mediator) : IRequestHandler<InsertImageCommand>
    {
        public async Task Handle(InsertImageCommand request, CancellationToken cancellationToken)
        {
            // Masuk tag
            await _Mediator.Send(new InsertTagCommand(){ Tags = request.Tags }, cancellationToken);

            // Masuk gambar
            var imageID = Guid.NewGuid();

            var image = new ImageModel(){
                Name = request.Name,
                UploadDate = DateTime.Now.SpecifyKind(),
                AgeRating = Domain.Constant.ModelConstant.AgeRating.GENERAL,
                ID = imageID,
                Description = "",
                Link = "",
                Source = ""
            };

            var imageTag = _DB.Set<TagModel>()
                .Where(x => request.Tags.Contains(x.Name))
                .Select(x => new ImageTagModel(){
                    ID = Guid.NewGuid(),
                    ImageID = imageID,
                    TagID = x.ID
                });
            
            await _DB.AddAsync(image, cancellationToken);
            await _DB.AddRangeAsync(image, cancellationToken);

            await _DB.SaveChangesAsync(cancellationToken);   
        }
    }
}