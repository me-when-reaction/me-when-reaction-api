using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhenAPI.Domain.Constant
{
    public static class FileConstant
    {
        /// <summary>
        /// 2 MB saja. Jika kebawah, maka akan dicoba untuk compress
        /// </summary>
        public const long MAX_FILESIZE_UPLOAD = 1024 * 1024 * 2;

        /// <summary>
        /// Max masuk ke filesystem (50KB). Jika gagal, suruh si uploader yang compress sendiri
        /// </summary>
        public const long MAX_FILESIZE_STORAGE = 1024 * 10;

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

        public static class StorageType
        {
            public const string Native = "Native";
            public const string Supabase = "Supabase";
            public const string S3 = "S3";
        }
    }
}