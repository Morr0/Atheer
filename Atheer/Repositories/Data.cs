using Atheer.Models;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class Data : DbContext
    {
        public Data(DbContextOptions<Data> options) : base(options)
        {
            // TODO enforce limits of types in Db thru UI, e.g. description of an article should be <= 512 chars
        }
        
        public DbSet<Article> Article { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<TagArticle> TagArticle { get; set; }
        
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureArticleModel(modelBuilder);
            ConfigureUserModel(modelBuilder);
            ConfigureTagsModels(modelBuilder);
        }
    }
}