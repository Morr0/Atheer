using System.Collections.Generic;
using Atheer.Services.FileService;
using Xunit;

namespace AtheerTests.UnitTests.UtilitiesTesting
{
    public class FileServiceUtilitiesTests
    {
        public static IEnumerable<object[]> GetKeyParameters()
        {
            yield return new object[]
            {
                FileUse.None, "hello", $"{FileServiceUtilities.NoneDir}/hello"
            };
            yield return new object[]
            {
                FileUse.UserImage, "world", $"{FileServiceUtilities.UserImageDir}/world"
            };
        }
        
        [Theory]
        [MemberData(nameof(GetKeyParameters))]
        public void GetKeyShouldCorrectGetKey(FileUse fileUse, string fileId, string expectedKey)
        {
            string key = FileServiceUtilities.GetKey(fileUse, fileId);

            Assert.Equal(expectedKey, key);
        }

        [Fact]
        public void GetFileUrlShouldContainCorrectS3Endpoints()
        {
            string bucketName = "someBucket";
            string key = "somKey/hello";

            string url = FileServiceUtilities.GetFileUrl(bucketName, ref key);
            
            Assert.Contains("https://", url);
            Assert.Contains(".s3.amazonaws.com/", url);
        }

        [Fact]
        public void GetCdnFileUrlShouldUseCorrectEndpoint()
        {
            string cdnUrl = "kfgjmhfgjhfgjf.somecdn.khtg.com";
            string key = "pep/ii";

            string url = FileServiceUtilities.GetCdnFileUrl(cdnUrl, ref key);

            Assert.Contains(cdnUrl, url);
            Assert.Contains(key, url);
        }

        public static IEnumerable<object[]> GetUrlParameters()
        {
            yield return new object[]
            {
                "someBucket", "h/t/a", "https://someBucket.s3.amazonaws.com/h/t/a"
            };
        }

        [Theory]
        [MemberData(nameof(GetUrlParameters))]
        public void GetFileUrlShouldGetCorrectUrl(string bucketName, string key, string expectedUrl)
        {
            string k = key;
            string url = FileServiceUtilities.GetFileUrl(bucketName, ref key);

            Assert.Equal(expectedUrl, url);
        }
    }
}