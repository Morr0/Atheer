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
        private readonly IAmazonS3 _s3Client;
        private S3 _s3Config;

        public FileService(IOptions<S3> s3Config, IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
            _s3Config = s3Config.Value;
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
                CannedACL = S3CannedACL.PublicRead,
                AutoCloseStream = false
            };
            
            await _s3Client.PutObjectAsync(putObjectRequest).ConfigureAwait(false);
            
            return FileServiceUtilities.GetFileUrl(_s3Config.BucketName, ref s3Key);
        }

        public Task Remove(FileUse fileUse, string fileId)
        {
            string s3Key = FileServiceUtilities.GetKey(fileUse, fileId);
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _s3Config.BucketName,
                Key = s3Key
            };

            return _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}