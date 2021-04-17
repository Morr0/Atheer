using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Atheer.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3 _s3Config;

        private readonly AsyncRetryPolicy _retryPolicy;

        public FileService(IOptions<S3> s3Config, IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
            _s3Config = s3Config.Value;

            _retryPolicy = Policy.Handle<AmazonS3Exception>().RetryAsync(3);
        }
        
        public async Task<string> Add(FileUse fileUse, string fileId, string contentType, Stream stream)
        {
            string s3Key = FileServiceUtilities.GetKey(fileUse, fileId);
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = s3Key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.Private,
                AutoCloseStream = false
            };

            await _retryPolicy.ExecuteAsync(() => _s3Client.PutObjectAsync(putObjectRequest)).ConfigureAwait(false);

            return FileServiceUtilities.GetCdnFileUrl(_s3Config.CdnUrl, s3Key);
        }

        public Task Remove(FileUse fileUse, string fileId)
        {
            string s3Key = FileServiceUtilities.GetKey(fileUse, fileId);
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = s3Key
            };

            return _retryPolicy.ExecuteAsync(() => _s3Client.DeleteObjectAsync(deleteObjectRequest));
        }

        public string GetCdnUrlFromFileKey(string fileKey)
        {
            return FileServiceUtilities.GetCdnFileUrl(_s3Config.CdnUrl, fileKey);
        }
    }
}