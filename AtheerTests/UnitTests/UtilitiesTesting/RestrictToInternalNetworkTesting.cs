using Atheer.Controllers.Utilities.Filters;
using Xunit;

namespace AtheerTests.UnitTests.UtilitiesTesting
{
    public class RestrictToInternalNetworkTesting
    {
        [Theory]
        [InlineData("127.0.0.1", true)]
        [InlineData("127.0.0.0", false)]
        [InlineData("10.0.1.131", true)]
        [InlineData("8.1.1.131", false)]
        [InlineData("172.111.102.131", false)]
        [InlineData("172.18.1.131", true)]
        [InlineData("192.168.1.1", true)]
        [InlineData("191.168.0.1", false)]
        public void TestInternalIpv4Address(string ipAddress, bool expectedResult)
        {
            bool actualResult = RestrictToInternalNetwork.InternalIpv4Address(ipAddress);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}