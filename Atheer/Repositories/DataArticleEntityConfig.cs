using Atheer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Atheer.Repositories
{
    public partial class Data : DbContext
    {
        private void ConfigureArticleModel(ModelBuilder modelBuilder)
        {
            // Key
            modelBuilder.Entity<Article>()
                .HasKey(x => new
                {
                    x.CreatedYear,
                    x.TitleShrinked
                });

            // Properties
            // - Booleans
            modelBuilder.Entity<Article>()
                .Property(x => x.Likeable)
                .HasDefaultValue(false);
            modelBuilder.Entity<Article>()
                .Property(x => x.Shareable)
                .HasDefaultValue(false);
            modelBuilder.Entity<Article>()
                .Property(x => x.Unlisted)
                .HasDefaultValue(false);
            modelBuilder.Entity<Article>()
                .Property(x => x.Draft)
                .HasDefaultValue(false);

            // - Strings
            modelBuilder.Entity<Article>()
                .Property(x => x.Content)
                .HasColumnType("text");
            modelBuilder.Entity<Article>()
                .Property(x => x.Description)
                .HasColumnType("varchar(512)");
            modelBuilder.Entity<Article>()
                .Property(x => x.Title)
                .HasColumnType("text");
            modelBuilder.Entity<Article>()
                .Property(x => x.AuthorId)
                .HasColumnType("varchar(64)");
            modelBuilder.Entity<Article>()
                .Property(x => x.TitleShrinked)
                .HasColumnType("text");
            // - Numbers
            modelBuilder.Entity<Article>()
                .Property(x => x.Likes)
                .HasDefaultValue(0);
            modelBuilder.Entity<Article>()
                .Property(x => x.Shares)
                .HasDefaultValue(0);
            
            // Search
            //  - Refer to: https://www.npgsql.org/efcore/mapping/full-text-search.html?tabs=pg12%2Cv5
            modelBuilder.Entity<Article>()
                .HasGeneratedTsVectorColumn(x => x.SearchVector, "english", x => new
                {
                    x.Title,
                    x.Description,
                    x.Content
                })
                .HasIndex(x => x.SearchVector)
                .HasMethod("GIN");

            modelBuilder.Entity<Article>(o =>
            {
                o.HasOne(x => x.Series)
                    .WithMany(x => x.Articles)
                    .HasForeignKey(x => x.SeriesId)
                    .IsRequired(false);
            });
        }

        private void ConfigureArticleSeries(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleSeries>(o =>
            {
                o.HasKey(x => x.Id);
                o.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                o.HasIndex(x => x.Finished)
                    .HasFilter($"\"{nameof(Atheer.Models.ArticleSeries.Finished)}\" IS FALSE");
            });
        }
    }
}