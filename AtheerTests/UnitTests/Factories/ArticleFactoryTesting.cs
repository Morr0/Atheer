using System;
using System.Globalization;
using Atheer.Controllers.ViewModels;
using Atheer.Extensions;
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
            var date = DateTime.UtcNow;
            var originalDate = date;
            string proposedSchedule = date.AddDays(1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, ref date, out bool scheduled, out DateTime scheduledSinceUtc);
            
            Assert.True(scheduled);
            Assert.Equal(DateTimeKind.Utc, scheduledSinceUtc.Kind);
            Assert.Equal(DateTimeKind.Utc, originalDate.Kind);
            Assert.Equal(originalDate, scheduledSinceUtc);
        }
        
        [Fact]
        public void SchedulerShouldNotScheduleOnTheSameDay()
        {
            var date = new DateTime(2000, 2, 1);
            var originalDate = date;
            string proposedSchedule = date.AddMinutes(1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, ref date, out bool scheduled, out DateTime scheduledSinceUtc);
            
            Assert.False(scheduled);
            Assert.Equal(originalDate, scheduledSinceUtc);
        }
        
        [Fact]
        public void SchedulerShouldNotScheduleOnSameTime()
        {
            var date = new DateTime(2000, 2, 1);
            string proposedSchedule = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            
            _factory.GetDate(proposedSchedule, ref date, out bool scheduled, out DateTime scheduledSinceUtc);
            
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
                Schedule = releaseDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
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
    }
}