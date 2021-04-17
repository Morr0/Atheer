using System;
using System.Runtime.CompilerServices;

namespace Atheer.Services.FileService
{
    public static class FileServiceUtilities
    {
        internal static readonly string NoneDir = "None";
        internal static readonly string UserImageDir = "UserImage";
        internal static readonly string ArticleNarrationDir = "ArticleNarration";
        
        internal static string GetKey(FileUse fileUse, string fileId)
        {
            switch (fileUse)
            {
                case FileUse.UserImage:
                    return $"{UserImageDir}/{fileId}";
                case FileUse.ArticleNarration:
                    return ArticleNarrationDir;
                default:
                    return $"{NoneDir}/{fileId}";
            }
        }
        
        internal static string GetFileUrl(string bucketName, string key)
        {
            string endpoint = $"https://{bucketName}.s3.amazonaws.com/{key}";
            return endpoint;
        }

        internal static string GetCdnFileUrl(string cdnUrl, ref string key)
        {
            string endpoint = $"{cdnUrl}/{key}";
            return endpoint;
        }
    }
}