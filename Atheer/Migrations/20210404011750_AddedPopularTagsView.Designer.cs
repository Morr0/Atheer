﻿// <auto-generated />
using System;
using Atheer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Atheer.Migrations
{
    [DbContext(typeof(Data))]
    [Migration("20210404011750_AddedPopularTagsView")]
    partial class AddedPopularTagsView
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Atheer.Models.Article", b =>
                {
                    b.Property<int>("CreatedYear")
                        .HasColumnType("integer");

                    b.Property<string>("TitleShrinked")
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("CreationDate")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(512)");

                    b.Property<bool>("Draft")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("LastUpdatedDate")
                        .HasColumnType("varchar(20)");

                    b.Property<bool>("Likeable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("Likes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<bool>("Scheduled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("ScheduledSinceDate")
                        .HasColumnType("varchar(20)");

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "english")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "Title", "Description", "Content" });

                    b.Property<bool>("Shareable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("Shares")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<bool>("Unlisted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.HasKey("CreatedYear", "TitleShrinked");

                    b.HasIndex("Scheduled")
                        .HasFilter("\"Scheduled\" IS TRUE");

                    b.HasIndex("SearchVector")
                        .HasMethod("GIN");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("Atheer.Models.NavItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NavItems");
                });

            modelBuilder.Entity("Atheer.Models.Tag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DateCreated")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("DateLastAddedTo")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("Atheer.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .HasColumnType("varchar(512)");

                    b.Property<string>("DateCreated")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("DateLastLoggedIn")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(192)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OAuthLogicalId")
                        .HasColumnType("varchar(48)");

                    b.Property<string>("OAuthProvider")
                        .HasColumnType("varchar(16)");

                    b.Property<bool>("OAuthUser")
                        .HasColumnType("boolean");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Roles")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Atheer.Repositories.Junctions.TagArticle", b =>
                {
                    b.Property<string>("TagId")
                        .HasColumnType("text");

                    b.Property<int>("ArticleCreatedYear")
                        .HasColumnType("integer");

                    b.Property<string>("ArticleTitleShrinked")
                        .HasColumnType("text");

                    b.HasKey("TagId", "ArticleCreatedYear", "ArticleTitleShrinked");

                    b.HasIndex("ArticleCreatedYear", "ArticleTitleShrinked");

                    b.ToTable("TagArticle");
                });

            modelBuilder.Entity("Atheer.Repositories.Views.PgStatActivity", b =>
                {
                    b.Property<string>("State")
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.ToView("pg_stat_activity");
                });

            modelBuilder.Entity("Atheer.Repositories.Views.PopularTag", b =>
                {
                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.ToView("popular_tags_view");
                });

            modelBuilder.Entity("Atheer.Repositories.Junctions.TagArticle", b =>
                {
                    b.HasOne("Atheer.Models.Tag", "Tag")
                        .WithMany("Tags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Atheer.Models.Article", "Article")
                        .WithMany("Tags")
                        .HasForeignKey("ArticleCreatedYear", "ArticleTitleShrinked")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Atheer.Models.Article", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Atheer.Models.Tag", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}