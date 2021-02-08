using Atheer.Models;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class  Data : DbContext
    {
        private void ConfigureTagsModels(ModelBuilder modelBuilder)
        {
            // Keys
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
                .HasForeignKey(nameof(Models.TagArticle.TagId));
            modelBuilder.Entity<TagArticle>()
                .HasOne(ta => ta.Article)
                .WithMany(a => a.Tags)
                .HasForeignKey(x => new
                {
                    x.ArticleCreatedYear,
                    x.ArticleTitleShrinked
                });
            
            // Properties
            // - Strings
            modelBuilder.Entity<Tag>()
                .Property(x => x.Title)
                .HasColumnType("varchar(64)");
            modelBuilder.Entity<Tag>()
                .Property(x => x.DateCreated)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<Tag>()
                .Property(x => x.DateLastAddedTo)
                .HasColumnType("varchar(20)");
        }
    }
}