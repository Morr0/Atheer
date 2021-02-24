using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;

namespace Atheer.Services.FileService
{
    public class FileService : IFileService
    {
        private S3 _s3Config;

        private const string NoneDir = "None";
        private const string UserImageDir = "UserImage";

        public FileService(IOptions<S3> s3Config)
        {
            _s3Config = s3Config.Value;
        }
        
        public async Task<string> Add(FileUse fileUse, string fileId, string contentType, Stream stream)
        {
            using var s3Client = new AmazonS3Client();
            
            string s3Key = GetKey(fileUse, fileId);
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = s3Key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead,
                AutoCloseStream = false
            };
            
            await s3Client.PutObjectAsync(putObjectRequest).ConfigureAwait(false);
            
            return GetFileUrl(ref s3Key);
        }

        internal string GetKey(FileUse fileUse, string fileId)
        {
            switch (fileUse)
            {
                case FileUse.UserImage:
                    return $"{UserImageDir}/{fileId}";
                default:
                    return $"{NoneDir}/{fileId}";
            }
        }

        internal string GetFileUrl(ref string key)
        {
            // putObjectResponse.
            string endpoint = $"https://{_s3Config.BucketName}.s3.amazonaws.com/{key}";
            return endpoint;
        }
    }
}