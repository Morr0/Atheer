﻿// <auto-generated />
using System.Collections.Generic;
using Atheer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Atheer.Migrations
{
    [DbContext(typeof(Data))]
    partial class DataModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("CreationDate")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("Draft")
                        .HasColumnType("boolean");

                    b.Property<string>("LastUpdatedDate")
                        .HasColumnType("text");

                    b.Property<bool>("Likeable")
                        .HasColumnType("boolean");

                    b.Property<int>("Likes")
                        .HasColumnType("integer");

                    b.Property<bool>("Scheduled")
                        .HasColumnType("boolean");

                    b.Property<string>("ScheduledSinceDate")
                        .HasColumnType("text");

                    b.Property<bool>("Shareable")
                        .HasColumnType("boolean");

                    b.Property<int>("Shares")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<List<string>>("Topics")
                        .HasColumnType("text[]");

                    b.Property<bool>("Unlisted")
                        .HasColumnType("boolean");

                    b.HasKey("CreatedYear", "TitleShrinked");

                    b.HasIndex("Scheduled")
                        .HasFilter("\"Scheduled\" IS TRUE");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("Atheer.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .HasColumnType("text");

                    b.Property<string>("DateCreated")
                        .HasColumnType("text");

                    b.Property<string>("DateLastLoggedIn")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Roles")
                        .HasColumnType("text");

                    b.Property<string>("VerificationDate")
                        .HasColumnType("text");

                    b.Property<bool>("Verified")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Verified")
                        .HasFilter("\"Verified\" IS FALSE");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
