using Atheer.Models;
using Atheer.Repositories.Junctions;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class Data : DbContext
    {
        public Data(DbContextOptions<Data> options) : base(options)
        {
            
        }
        
        public DbSet<Article> Article { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<TagArticle> TagArticle { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<ArticleSeries> ArticleSeries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureArticleModel(modelBuilder);
            ConfigureUserModel(modelBuilder);
            ConfigureTagsModels(modelBuilder);
            ConfigureNavItems(modelBuilder);
            ConfigureArticleSeries(modelBuilder);
            
            ConfigureViews(modelBuilder);
        }
    }
}