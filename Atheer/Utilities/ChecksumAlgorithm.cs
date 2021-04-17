using System;
using System.Security.Cryptography;
using System.Text;

namespace Atheer.Utilities
{
    public static class ChecksumAlgorithm
    {
        public static string ComputeMD5Checksum(string raw)
        {
            using var md5 = new MD5CryptoServiceProvider();
            var rawBytes = Encoding.Default.GetBytes(raw);
            var checksumBytes = md5.ComputeHash(rawBytes);
            return Convert.ToHexString(checksumBytes);
        }
    }
}