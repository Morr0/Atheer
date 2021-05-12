using System;
using System.Globalization;
using Atheer.Controllers.Article.Requests;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService;
using Atheer.Services.Utilities.TimeService;
using Atheer.Utilities;
using Atheer.Utilities.Markdown;
using AutoMapper;
using Markdig;
using Xunit;

namespace AtheerTests.UnitTests.Factories
{
    public class ArticleFactoryTesting
    {
        private IMapper _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile(typeof(ModelMappingsProfile))));
        private ArticleFactory _factory;
        
        // Default values for testing different things
        private const string UserId = "bob";
        private const string Content = "kjhgdjhdfkjgjdfghjdfhgdf";
        private const string Description = "ksjdghjkhjkgh";
        private const string Title = "Ttttttttt";

        public ArticleFactoryTesting()
        {
            _factory = new ArticleFactory(_mapper, new MarkdownPipelineBuilder().UseAdvancedExtensions()
                .UseBootstrap().Use<MarkdownExtension>().Build(), new TimeService());
        }
        
        [Fact]
        public void ShouldCreateAnArticle()
        {
            // Arrange
            var request = new AddArticleRequest
            {
                Content = Content,
                Description = Description,
                Title = Title
            };

            // Act
            var article = _factory.Create(request, UserId);

            // Assert
            Assert.Equal(request.Content, article.Content);
            Assert.Equal(request.Title, article.Title);
            Assert.Equal(request.Description, article.Description);
            Assert.Equal(UserId, article.AuthorId);
            Assert.True(article.EverPublished);
        }

        [Fact]
        public void ShouldCreateASeries()
        {
            string title = "Some title";
            string description = "Some des";
            string userId = "kk";

            var series = _factory.CreateSeries(userId, title, description);

            Assert.Equal(userId, series.AuthorId);
            Assert.Equal(title, series.Title);
            Assert.Equal(description, series.Description);
            Assert.False(series.Finished);
        }

        [Fact]
        public void ShouldSetSeriesFinished()
        {
            var series = new ArticleSeries();

            _factory.FinishSeries(series);

            Assert.True(series.Finished);
        }
    }
}