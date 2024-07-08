using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileTypeChecker;
using FluentValidation;
using MeWhenAPI.Domain.Constant;

namespace MeWhenAPI.Domain.Validator
{
    public class FormFileValidator : AbstractValidator<IFormFile>
    {
        public FormFileValidator()
        {
            var extension = FileConstant.Extension.Select(x => x.Value);

            RuleFor(x => x.Length).LessThanOrEqualTo(FileConstant.MAX_FILESIZE_STORAGE)
                .WithMessage($"File size must be less than {FileConstant.MAX_FILESIZE_STORAGE / 1024} KB,");

            RuleFor(x => x.OpenReadStream())
                .Must((obj, stream) =>
                {
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