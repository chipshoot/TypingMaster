﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TypingMaster.DataAccess.Data;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250318042427_AddConcurrencyToken")]
    partial class AddConcurrencyToken
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.AccountDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("GoalStats")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountEmail")
                        .IsUnique();

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.CourseDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LessonDataUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SettingsJson")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("courses", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.DrillStatsDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Accuracy")
                        .HasColumnType("double precision");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("KeyEventsJson")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("LessonId")
                        .HasColumnType("integer");

                    b.Property<int>("PracticeLogId")
                        .HasColumnType("integer");

                    b.Property<string>("PracticeText")
                        .HasColumnType("text");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TrainingType")
                        .HasColumnType("integer");

                    b.Property<string>("TypedText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Wpm")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PracticeLogId");

                    b.ToTable("drill_stats", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.PracticeLogDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("integer");

                    b.Property<Guid>("CurrentCourseId")
                        .HasColumnType("uuid");

                    b.Property<int>("CurrentLessonId")
                        .HasColumnType("integer");

                    b.Property<string>("KeyStatsJson")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("PracticeDuration")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("practices", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.UserProfileDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("user_profiles", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.CourseDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", null)
                        .WithMany("Courses")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.DrillStatsDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.PracticeLogDao", null)
                        .WithMany("PracticeStats")
                        .HasForeignKey("PracticeLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.PracticeLogDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", null)
                        .WithOne("History")
                        .HasForeignKey("TypingMaster.DataAccess.Dao.PracticeLogDao", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.UserProfileDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", null)
                        .WithOne("User")
                        .HasForeignKey("TypingMaster.DataAccess.Dao.UserProfileDao", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.AccountDao", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("History")
                        .IsRequired();

                    b.Navigation("User")
                        .IsRequired();
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.PracticeLogDao", b =>
                {
                    b.Navigation("PracticeStats");
                });
#pragma warning restore 612, 618
        }
    }
}
