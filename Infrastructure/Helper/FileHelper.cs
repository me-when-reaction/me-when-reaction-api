using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using MeWhen.Domain.Configuration;

namespace MeWhen.Infrastructure.Helper
{
    public class FileHelper
    {
        public async static Task<bool> UploadImage(string fileName, Stream content, CancellationToken? token = null)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;
                var storageConfig = Program.Config.GetSection("Supabase:Storage").Get<StorageConfiguration>() ?? throw new Exception("S3 Storage not found");
                var supabaseConfig = Program.Config.GetSection("Supabase:Service").Get<SupabaseConfiguration>() ?? throw new Exception("Supabase service not found");

                #region Native
                using var stream = File.Create(finalPath);

                if (token != null) await content.CopyToAsync(stream, token ?? default);
                else await content.CopyToAsync(stream);
                #endregion

                // #region S3
                // var awsClient = new AmazonS3Client(
                //     new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
                //     new AmazonS3Config() {
                //         ServiceURL = storageConfig.Endpoint,
                //         ForcePathStyle = true
                //     }
                // );
                // var transferUtility = new TransferUtility(awsClient);
                // await transferUtility.UploadAsync(new TransferUtilityUploadRequest()
                // {
                //     InputStream = content,
                //     Key = fileName,
                //     BucketName = storageConfig.Bucket,
                //     CannedACL = S3CannedACL.PublicRead
                // });
                // #endregion

                // #region Supabase
                // var supabase = await new Supabase.Client(
                //     supabaseConfig.URL,
                //     supabaseConfig.Key,
                //     new()
                //     {
                //         AutoConnectRealtime = true
                //     }
                // ).InitializeAsync();

                // await supabase.Storage
                //     .From(storageConfig.Bucket)
                //     .Upload(content.ToByteArray(), fileName, new()
                //     {
                //         CacheControl = "3600",
                //         Upsert = false
                //     });
                // #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async static Task<bool> DeleteImage(string fileName)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;
                var storageConfig = Program.Config.GetSection("Supabase:Storage").Get<StorageConfiguration>() ?? throw new Exception("S3 Storage not found");
                var supabaseConfig = Program.Config.GetSection("Supabase:Service").Get<SupabaseConfiguration>() ?? throw new Exception("Supabase service not found");

                #region Native
                await Task.Run(() => { if (File.Exists(finalPath)) File.Delete(finalPath); });
                #endregion
                
                // #region S3
                // var awsClient = new AmazonS3Client(
                //     new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
                //     new AmazonS3Config() {
                //         ServiceURL = storageConfig.Endpoint,
                //         ForcePathStyle = true
                //     }
                // );
                // await awsClient.DeleteObjectAsync(new() {
                //     BucketName = storageConfig.Bucket,
                //     Key = fileName
                // });
                // #endregion

                // #region Supabase
                // var supabase = await new Supabase.Client(
                //     supabaseConfig.URL,
                //     supabaseConfig.Key,
                //     new()
                //     {
                //         AutoConnectRealtime = true
                //     }
                // ).InitializeAsync();
                // await supabase.Storage
                //     .From(storageConfig.Bucket)
                //     .Remove(fileName);
                // #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}