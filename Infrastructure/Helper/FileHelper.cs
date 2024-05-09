using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhen.Infrastructure.Helper
{
    public class FileHelper
    {
        public async static Task<bool> UploadFile(string fileName, Stream content, CancellationToken? token = null)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;

                using var stream = File.Create(finalPath);

                if (token != null) await content.CopyToAsync(stream, token ?? default);
                else await content.CopyToAsync(stream); 

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteFile(string fileName)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;
                if (File.Exists(finalPath)) File.Delete(finalPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}