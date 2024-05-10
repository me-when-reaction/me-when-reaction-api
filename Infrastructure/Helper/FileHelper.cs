using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using MeWhen.Domain.Configuration;

namespace MeWhen.Infrastructure.Helper
{
    public class FileHelper
    {
        public async static Task<bool> UploadFile(string fileName, Stream content, CancellationToken? token = null)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;

                // using var stream = File.Create(finalPath);

                // if (token != null) await content.CopyToAsync(stream, token ?? default);
                // else await content.CopyToAsync(stream);

                // S3

                var storageConfig = Program.Config.GetSection("Supabase:Storage").Get<StorageConfiguration>() ?? throw new Exception("S3 Storage not found");
                var awsClient = new AmazonS3Client(
                    new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
                    new AmazonS3Config() {
                        ServiceURL = storageConfig.Endpoint,
                        ForcePathStyle = true
                    }
                );
                var transferUtility = new TransferUtility(awsClient);
                await transferUtility.UploadAsync(new TransferUtilityUploadRequest()
                {
                    InputStream = content,
                    Key = fileName,
                    BucketName = storageConfig.Bucket,
                    CannedACL = S3CannedACL.PublicRead
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async static Task<bool> DeleteFile(string fileName)
        {
            try
            {
                var finalPath = Program.Config.GetValue<string>("TempStorage") + "/" + fileName;
                if (File.Exists(finalPath)) File.Delete(finalPath);

                // S3
                var storageConfig = Program.Config.GetSection("Supabase:Storage").Get<StorageConfiguration>() ?? throw new Exception("S3 Storage not found");
                var awsClient = new AmazonS3Client(
                    new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
                    new AmazonS3Config() {
                        ServiceURL = storageConfig.Endpoint,
                        ForcePathStyle = true
                    }
                );
                await awsClient.DeleteObjectAsync(new() {
                    BucketName = storageConfig.Bucket,
                    Key = fileName
                });
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}