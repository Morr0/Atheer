using Atheer.Models;
using Atheer.Repositories.Junctions;
using Atheer.Repositories.Views;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class  Data : DbContext
    {
        public virtual DbSet<PopularTag> PopularTag { get; set; }
        
        private void ConfigureTagsModels(ModelBuilder modelBuilder)
        {
            // Views
            modelBuilder.Entity<PopularTag>().HasNoKey().ToView("popular_tags_view");
            
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
                .HasForeignKey(nameof(Junctions.TagArticle.TagId));
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