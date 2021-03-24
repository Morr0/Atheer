using Atheer.Repositories.Views;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class Data
    {
        public virtual DbSet<PgStatActivity> PgStatActivity { get; set; }

        private void ConfigureViews(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PgStatActivity>()
                .HasNoKey()
                .ToView("pg_stat_activity");
            
            modelBuilder.Entity<PgStatActivity>(x =>
            {
                x.Property(x => x.State)
                    .HasColumnName("state");
            });
        }
    }
}