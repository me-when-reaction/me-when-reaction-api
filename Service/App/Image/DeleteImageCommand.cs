using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Exception;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using MeWhenAPI.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MeWhenAPI.Service.App.Image
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

    public class DeleteImageCommandHandler(MeWhenDBContext _DB, IFileUtilities _File) : IRequestHandler<DeleteImageCommand>
    {
        public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var image = _DB.Set<ImageModel>().FirstOrDefault(x => x.ID == request.ID)
                ?? throw new BadRequestException("Image not found");

            var link = $"{image.ID}.{image.Extension}";
            _DB.Remove(image);
            await _DB.SaveChangesAsync(cancellationToken);
            await _File.DeleteImage(link);
        }
    }
}