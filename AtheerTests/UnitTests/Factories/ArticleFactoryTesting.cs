using System;
using System.Globalization;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService;
using Atheer.Utilities;
using AutoMapper;
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
            _factory = new ArticleFactory(_mapper);
        }
        
        [Fact]
        public void ShouldCreateAnArticle()
        {
            // Arrange
            var dto = new ArticleEditViewModel
            {
                Content = Content,
                Description = Description,
                Title = Title,
                AuthorId = UserId,
            };

            // Act
            var article = _factory.Create(ref dto, UserId);

            // Assert
            Assert.Equal(dto.Content, article.Content);
            Assert.Equal(dto.Title, article.Title);
            Assert.Equal(dto.Description, article.Description);
            Assert.Equal(UserId, article.AuthorId);
            Assert.False(article.Scheduled);
        }

        [Fact]
        public void SchedulerShouldBeRight()
        {
            string proposedSchedule = DateTime.UtcNow.AddDays(1).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, out var now, out bool scheduled, out string scheduledSince);
            
            Assert.True(scheduled);
        }
        
        [Fact]
        public void SchedulerShouldNotScheduleOnTheSameDay()
        {
            var date = new DateTime(2000, 2, 1);
            string proposedSchedule = date.AddMinutes(1).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, out var now, out bool scheduled, out string scheduledSince);
            
            Assert.False(scheduled);
        }
        
        [Fact]
        public void SchedulerShouldNotScheduleOnSameTime()
        {
            var date = new DateTime(2000, 2, 1);
            string proposedSchedule = date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, out var now, out bool scheduled, out string scheduledSince);
            
            Assert.False(scheduled);
        }

        [Fact]
        public void ShouldCreateAnArticleThatIsScheduled()
        {
            // Arrange
            var now = DateTime.Now;
            var releaseDate = now.AddDays(1);
            var releaseDateUtc = releaseDate.ToUniversalTime();
            var dto = new ArticleEditViewModel
            {
                Content = Content,
                Description = Description,
                Title = Title,
                AuthorId = UserId,
                Schedule = releaseDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
            };

            // Act
            var article = _factory.Create(ref dto, UserId);
            var creationDate = DateTime.Parse(article.CreationDate);
            var scheduledSinceDate = DateTime.Parse(article.ScheduledSinceDate);

            // Assert
            Assert.True(article.Scheduled);
            Assert.NotEqual(article.ScheduledSinceDate, article.CreationDate);
            Assert.True(creationDate > scheduledSinceDate);
        }

        [Fact]
        public void ScheduledArticleShouldBeUnscheduled()
        {
            var now = DateTime.UtcNow;
            string scheduledSince = now.AddMinutes(-1).GetString();
            var article = new Article
            {
                Scheduled = true,
                ScheduledSinceDate = scheduledSince,
                CreationDate = now.AddMinutes(1).GetString()
            };
            
            _factory.Unschedule(article, now);
            
            Assert.False(article.Scheduled);
            Assert.Equal(now.GetString(), article.CreationDate);
        }

        [Fact]
        public void ScheduledArticleAlreadyReleasedShouldNotAffectAnythingIfUnscheduled()
        {
            var releaseDatetime = DateTime.UtcNow;
            string scheduledSince = releaseDatetime.AddMinutes(-1).GetString();
            var article = new Article
            {
                Scheduled = false,
                ScheduledSinceDate = scheduledSince,
                CreationDate = releaseDatetime.GetString()
            };

            var proposedReleaseDatetime = releaseDatetime.AddMinutes(1);
            _factory.Unschedule(article, proposedReleaseDatetime);
            
            Assert.False(article.Scheduled);
            Assert.Equal(releaseDatetime.GetString(), article.CreationDate);
        }
    }
}