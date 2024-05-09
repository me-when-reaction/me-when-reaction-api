using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhen.Domain.Constant
{
    public static class FileConstant
    {
        public const long MAX_SIZE = 1024 * 1024 * 10;
        public enum FileTypeEnum
        {
            PNG,
            JPG,
            JPEG,
            WEBP
        }
        public static readonly Dictionary<FileTypeEnum, string> MIME = new(){
            { FileTypeEnum.PNG, "image/png" },
            { FileTypeEnum.JPG, "image/jpg" },
            { FileTypeEnum.JPEG, "image/jpeg" },
            { FileTypeEnum.WEBP, "image/webp" },
        };

        public static readonly Dictionary<FileTypeEnum, string> Extension = new(){
            { FileTypeEnum.PNG, "png" },
            { FileTypeEnum.JPG, "jpg" },
            { FileTypeEnum.JPEG, "jpeg" },
            { FileTypeEnum.WEBP, "webp" },
        };
    }
}