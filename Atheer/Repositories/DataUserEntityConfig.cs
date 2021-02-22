using Atheer.Models;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Repositories
{
    public partial class Data : DbContext
    {
        private void ConfigureUserModel(ModelBuilder modelBuilder)
        {
            // Keys
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            
            // Indexes
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Verified)
                .HasFilter($"\"{nameof(Atheer.Models.User.Verified)}\" IS FALSE");
            
            // Properties
            // - Booleans
            modelBuilder.Entity<User>()
                .Property(x => x.Verified)
                .HasDefaultValue(false);
            // - Strings
            modelBuilder.Entity<User>()
                .Property(x => x.Bio)
                .HasColumnType("varchar(512)");
            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .HasColumnType("varchar(192)");
            modelBuilder.Entity<User>()
                .Property(x => x.FirstName)
                .HasColumnType("varchar(32)");
            modelBuilder.Entity<User>()
                .Property(x => x.LastName)
                .HasColumnType("varchar(32)");
            modelBuilder.Entity<User>()
                .Property(x => x.Roles)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.DateCreated)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<User>()
                .Property(x => x.DateLastLoggedIn)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<User>()
                .Property(x => x.VerificationDate)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.ImageUrl)
                .HasColumnType("text");
        }
    }
}