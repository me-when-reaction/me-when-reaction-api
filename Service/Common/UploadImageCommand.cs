using System;
using FluentValidation;
using MediatR;
using MeWhen.Domain.Validator;

namespace MeWhen.Service.Common
{
    public class UploadImageCommand : IRequest
    {
        public required IFormFile File { get; set; }
        
        /// <summary>
        /// Ada extension juga
        /// </summary>
        public required string FileName { get; set; }
    }

    public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            RuleFor(x => x.File).SetValidator(new IFormFileValidator());
            RuleFor(x => x.FileName).NotEmpty().WithMessage("Filename empty");
            RuleFor(x => x.FileName).Must(x => x.Split(".").Length == 2).WithMessage("Filename not valid");
        }
    }

    public class UploadImageCommandHandler(IConfiguration _Config) : IRequestHandler<UploadImageCommand>
    {
        public async Task Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var path = _Config.GetValue<string>("TempStorage");

            var finalPath = path + "/" + request.FileName;

            using var stream = File.Create(finalPath);
            await request.File.CopyToAsync(stream, cancellationToken);
        }
    }
}
