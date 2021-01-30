using Atheer.Models;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public class Data : DbContext
    {
        public Data(DbContextOptions<Data> options) : base(options)
        {
            
        }
        
        public DbSet<Article> Article { get; set; }
        
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureArticleModel(modelBuilder);
            ConfigureUserModel(modelBuilder);
            ConfigureTagsModels(modelBuilder);
        }

        private void ConfigureTagsModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TagArticle>()
                .HasKey(x => new
                {
                    x.TagId,
                    x.ArticleCreatedYear,
                    x.ArticleTitleShrinked
                });

            modelBuilder.Entity<TagArticle>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.Tags)
                .HasForeignKey(nameof(TagArticle.TagId));
            modelBuilder.Entity<TagArticle>()
                .HasOne(ta => ta.Article)
                .WithMany(a => a.Tags)
                .HasForeignKey(x => new
                {
                    x.ArticleCreatedYear,
                    x.ArticleTitleShrinked
                });
        }

        private void ConfigureUserModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Verified)
                .HasFilter($"\"{nameof(Atheer.Models.User.Verified)}\" IS FALSE");
        }

        private void ConfigureArticleModel(ModelBuilder modelBuilder)
        {
            // Primary key
            modelBuilder.Entity<Article>()
                .HasKey(x => new
                {
                    x.CreatedYear,
                    x.TitleShrinked
                });

            modelBuilder.Entity<Article>()
                .HasIndex(x => x.Scheduled)
                .HasFilter($"\"{nameof(Atheer.Models.Article.Scheduled)}\" IS TRUE");
        }
    }
}