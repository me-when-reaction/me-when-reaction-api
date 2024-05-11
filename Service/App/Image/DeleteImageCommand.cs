using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Exception;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;
using MeWhen.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;

namespace MeWhen.Service.App.Image
{
    public class DeleteImageCommand : IRequest
    {
        public required Guid ID { get; set; }
    }

    public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
    {
        public DeleteImageCommandValidator()
        {
            RuleFor(x => x.ID).NotEmpty();
        }
    }

    public class DeleteImageCommandHandler(MeWhenDBContext _DB) : IRequestHandler<DeleteImageCommand>
    {
        public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var image = _DB.Set<ImageModel>().FirstOrDefault(x => x.ID == request.ID)
                ?? throw new BadRequestException("Image not found");
            
            var link = $"{image.ID}.{image.Extension}";
            _DB.Remove(image);
            await _DB.SaveChangesAsync(cancellationToken);
            await FileHelper.DeleteImage(link);
        }
    }
}