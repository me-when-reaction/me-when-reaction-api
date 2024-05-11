using System;
using ImageMagick;
using MeWhen.Domain.Constant;
using MeWhen.Domain.Exception;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MeWhen.Domain.DTO
{
    public class CompressedImageDTO
    {
        public string FileName { get; set; }
        public string Extension => FileName.Split('.').Length == 2 ? FileName.Split('.', 2)[1] : "";
        public MemoryStream Content { get; set; }
        public long Length { get; set; } = 0;

        public CompressedImageDTO(IFormFile file)
        {
            FileName = file.FileName;
            Length = file.Length;

            using var stream = new MemoryStream();
            file.CopyToAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);
            Content = stream;

            // Try compress
            if (Length > FileConstant.MAX_FILESIZE_STORAGE)
            {
                using var newImage = new MagickImage(Content);
                newImage.Quality = 50;

                Content.SetLength(0);
                newImage.Write(Content);

                if (Content.Length > FileConstant.MAX_FILESIZE_STORAGE)
                    throw new BadRequestException("Sorry, we cannot compress the image 😞");
            }
        }
    }
}
