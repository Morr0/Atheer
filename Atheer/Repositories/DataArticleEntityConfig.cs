﻿using Atheer.Models;
using Microsoft.EntityFrameworkCore;

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

            // Indexes
            modelBuilder.Entity<Article>()
                .HasIndex(x => x.Scheduled)
                .HasFilter($"\"{nameof(Atheer.Models.Article.Scheduled)}\" IS TRUE");
            
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
            modelBuilder.Entity<Article>()
                .Property(x => x.Scheduled)
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
            modelBuilder.Entity<Article>()
                .Property(x => x.ScheduledSinceDate)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<Article>()
                .Property(x => x.CreationDate)
                .HasColumnType("varchar(20)");
            modelBuilder.Entity<Article>()
                .Property(x => x.LastUpdatedDate)
                .HasColumnType("varchar(20)");
            // - Numbers
            modelBuilder.Entity<Article>()
                .Property(x => x.Likes)
                .HasDefaultValue(0);
            modelBuilder.Entity<Article>()
                .Property(x => x.Shares)
                .HasDefaultValue(0);
        }
    }
}