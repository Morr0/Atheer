using Atheer.Services.TagService;
using Xunit;

namespace AtheerTests.UnitTests.Factories
{
    public class TagFactoryTesting
    {
        private TagFactory _factory = new TagFactory();
        
        [Theory]
        [InlineData("This", "this")]
        [InlineData("This is a title", "this-is-a-title")]
        [InlineData("", "")]
        public void ShouldCreateId(string title, string expectedId)
        {
            string actualId = _factory.GetId(title);
            
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void ShouldCreateTag()
        {
            string title = "Hello";
            string expectedId = "hello";

            var tag = _factory.CreateTag(title);
            
            Assert.Equal(expectedId, tag.Id);
            Assert.Equal(title, tag.Title);
        }
    }
}