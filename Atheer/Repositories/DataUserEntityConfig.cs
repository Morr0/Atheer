﻿using Atheer.Models;
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
                .HasIndex(x => x.Email);

            // Properties
            // - Booleans
            // - Strings
            modelBuilder.Entity<User>()
                .Property(x => x.Bio)
                .HasColumnType("varchar(512)");
            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .HasColumnType("varchar(192)");
            modelBuilder.Entity<User>()
                .Property(x => x.Roles)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.ImageUrl)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .Property(x => x.OAuthProvider)
                .HasColumnType("varchar(16)");
            modelBuilder.Entity<User>()
                .Property(x => x.OAuthLogicalId)
                .HasColumnType("varchar(48)");
        }
    }
}