using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeWhen.Domain.Configuration;
using MeWhen.Domain.Constant;
using MeWhen.Infrastructure.Helper;
using Microsoft.Extensions.Options;

namespace MeWhen.Infrastructure.Utilities
{
    public interface IFileUtilities
    {
        public Task UploadImage(string fileName, Stream content, CancellationToken? token = null);
        public Task DeleteImage(string fileName);
    }

    public class FileUtilities(IOptions<StorageConfiguration> _StorageConfig, Supabase.Client _Supabase) : IFileUtilities
    {
        public async Task DeleteImage(string fileName)
        {
            if (_StorageConfig.Value.StorageType == FileConstant.StorageType.Native)
            {
                var finalPath = _StorageConfig.Value.NativePath + "/" + fileName;
                #region Native
                await Task.Run(() => { if (File.Exists(finalPath)) File.Delete(finalPath); });
                #endregion
            }
            else if (_StorageConfig.Value.StorageType == FileConstant.StorageType.Supabase)
            {
                #region Supabase
                await _Supabase.Storage
                    .From(_StorageConfig.Value.Bucket)
                    .Remove(fileName);
                #endregion
            }
            // else if (_StorageConfig.Value.StorageType == FileConstant.StorageType.S3)
            // {
            //     #region S3
            //     var awsClient = new AmazonS3Client(
            //         new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
            //         new AmazonS3Config() {
            //             ServiceURL = storageConfig.Endpoint,
            //             ForcePathStyle = true
            //         }
            //     );
            //     await awsClient.DeleteObjectAsync(new() {
            //         BucketName = storageConfig.Bucket,
            //         Key = fileName
            //     });
            //     #endregion
            // }
            else throw new Exception("File storage not defined");
        }

        public async Task UploadImage(string fileName, Stream content, CancellationToken? token = null)
        {
            if (_StorageConfig.Value.StorageType == FileConstant.StorageType.Native)
            {
                #region Native
                var finalPath = _StorageConfig.Value.NativePath + "/" + fileName;
                using var stream = File.Create(finalPath);

                if (token != null) await content.CopyToAsync(stream, token ?? default);
                else await content.CopyToAsync(stream);
                #endregion
            }
            else if (_StorageConfig.Value.StorageType == FileConstant.StorageType.Supabase)
            {
                #region Supabase
                await _Supabase.Storage
                    .From(_StorageConfig.Value.Bucket)
                    .Upload(content.ToByteArray(), fileName, new()
                    {
                        CacheControl = "3600",
                        Upsert = false
                    });
                #endregion
            }
            // else if (_StorageConfig.Value.StorageType == FileConstant.StorageType.S3)
            // {
            //     #region S3
            //     var awsClient = new AmazonS3Client(
            //         new BasicAWSCredentials(storageConfig.ID, storageConfig.Secret),
            //         new AmazonS3Config() {
            //             ServiceURL = storageConfig.Endpoint,
            //             ForcePathStyle = true
            //         }
            //     );
            //     var transferUtility = new TransferUtility(awsClient);
            //     await transferUtility.UploadAsync(new TransferUtilityUploadRequest()
            //     {
            //         InputStream = content,
            //         Key = fileName,
            //         BucketName = storageConfig.Bucket,
            //         CannedACL = S3CannedACL.PublicRead
            //     });
            //     #endregion
            // }
            else throw new Exception("File storage not defined");
        }
    }
}