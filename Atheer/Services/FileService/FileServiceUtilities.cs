using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AtheerTests")]
namespace Atheer.Services.FileService
{
    public static class FileServiceUtilities
    {
        internal static readonly string NoneDir = "None";
        internal static readonly string UserImageDir = "UserImage";
        
        internal static string GetKey(FileUse fileUse, string fileId)
        {
            switch (fileUse)
            {
                case FileUse.UserImage:
                    return $"{UserImageDir}/{fileId}";
                default:
                    return $"{NoneDir}/{fileId}";
            }
        }

        internal static string GetFileUrl(string bucketName, ref string key)
        {
            string endpoint = $"https://{bucketName}.s3.amazonaws.com/{key}";
            return endpoint;
        }
    }
}