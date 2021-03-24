using Atheer.Models;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class Data
    {
        public DbSet<NavItem> NavItems { get; set; }

        private void ConfigureNavItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NavItem>(x =>
            {
                x.HasKey(x => x.Id);
                x.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                x.Property(x => x.Name)
                    .HasColumnType("varchar(32)");
                x.Property(x => x.Url)
                    .HasColumnType("text");
            });
        }
    }
}