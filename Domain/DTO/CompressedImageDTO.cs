using System;
using ImageMagick;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Exception;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MeWhenAPI.Domain.DTO
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

            var stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // stream.Seek(0, SeekOrigin.Begin);
            Content = stream;
            // Content = (MemoryStream)file.OpenReadStream();

            // Try compress
            if (Length > FileConstant.MAX_FILESIZE_STORAGE)
            {
                var newImage = new MagickImage(Content)
                {
                    Quality = 50
                };

                Content.SetLength(0);
                newImage.Write(Content);
                stream.Seek(0, SeekOrigin.Begin);

                if (Content.Length > FileConstant.MAX_FILESIZE_STORAGE)
                    throw new BadRequestException($"Sorry, we cannot compress the image down to {FileConstant.MAX_FILESIZE_STORAGE / 1024}KB 😞");
            }
        }
    }
}
