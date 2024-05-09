using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileTypeChecker;
using FluentValidation;
using MeWhen.Domain.Constant;

namespace MeWhen.Domain.Validator
{
    public class IFormFileValidator : AbstractValidator<IFormFile>
    {
        public IFormFileValidator()
        {
            var extension = FileConstant.Extension.Select(x => x.Value);

            RuleFor(x => x.Length).LessThanOrEqualTo(FileConstant.MAX_SIZE)
                .WithMessage($"File size must be less than {FileConstant.MAX_SIZE / 1024 / 1024} MB");

            RuleFor(x => x.OpenReadStream())
                .Must((obj, stream) => {
                    if (stream.Length == 0 || !FileTypeValidator.IsTypeRecognizable(stream)) return false;
                    return 
                        extension.Contains(FileTypeValidator.GetFileType(stream).Extension)
                        && FileTypeValidator.IsImage(stream)
                        && extension.Contains(Path.GetExtension(obj.FileName)[1..]);
                })
                .WithMessage($"File format must be ${string.Join(", ", extension)}");
        }
    }
}