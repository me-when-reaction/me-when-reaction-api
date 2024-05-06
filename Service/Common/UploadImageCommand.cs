using System;
using FluentValidation;
using MediatR;

namespace MeWhen.Service.Common
{
    public class UploadImageCommand : IRequest
    {
        public required string FileName { get; set; }
        public required string Extension { get; set; }
        public required MemoryStream Content { get; set; } 
    }

    public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            
        }
    }

    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand>
    {
        public Task Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
