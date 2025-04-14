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
    [Migration("20250414163757_FixModelChanges")]
    partial class FixModelChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.AccountDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountEmail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_email");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_name");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("GoalStats")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("goal_stats");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer")
                        .HasColumnName("version");

                    b.HasKey("Id");

                    b.HasIndex("AccountEmail")
                        .IsUnique();

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.CourseDao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("LessonDataUrl")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("lesson_data_url");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("SettingsJson")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("settings_json");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("courses", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.DrillStatsDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Accuracy")
                        .HasColumnType("double precision")
                        .HasColumnName("accuracy");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid")
                        .HasColumnName("course_id");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("finish_time");

                    b.Property<string>("KeyEventsJson")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("key_events_json");

                    b.Property<int>("LessonId")
                        .HasColumnType("integer")
                        .HasColumnName("lesson_id");

                    b.Property<int>("PracticeLogId")
                        .HasColumnType("integer")
                        .HasColumnName("practice_log_id");

                    b.Property<string>("PracticeText")
                        .HasColumnType("text")
                        .HasColumnName("practice_text");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_time");

                    b.Property<int>("TrainingType")
                        .HasColumnType("integer")
                        .HasColumnName("training_type");

                    b.Property<string>("TypedText")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("typed_text");

                    b.Property<int>("Wpm")
                        .HasColumnType("integer")
                        .HasColumnName("wpm");

                    b.HasKey("Id");

                    b.HasIndex("PracticeLogId");

                    b.ToTable("drill_stats", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.LoginCredentialDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("ExternalIdpId")
                        .HasColumnType("text")
                        .HasColumnName("external_idp_id");

                    b.Property<string>("ExternalIdpType")
                        .HasColumnType("text")
                        .HasColumnName("external_idp_type");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<DateTime>("LastLoginAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_login_at");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("login_credentials", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.LoginLogDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<string>("FailureReason")
                        .HasColumnType("text")
                        .HasColumnName("failure_reason");

                    b.Property<string>("IpAddress")
                        .HasColumnType("text")
                        .HasColumnName("ip_address");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("boolean")
                        .HasColumnName("is_successful");

                    b.Property<DateTime>("LoginTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("login_time");

                    b.Property<string>("UserAgent")
                        .HasColumnType("text")
                        .HasColumnName("user_agent");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("login_logs", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.PracticeLogDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<Guid>("CurrentCourseId")
                        .HasColumnType("uuid")
                        .HasColumnName("current_course_id");

                    b.Property<int>("CurrentLessonId")
                        .HasColumnType("integer")
                        .HasColumnName("current_lesson_id");

                    b.Property<string>("KeyStatsJson")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("key_stats_json");

                    b.Property<long>("PracticeDuration")
                        .HasColumnType("bigint")
                        .HasColumnName("practice_duration");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("practices", (string)null);
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.UserProfileDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("integer")
                        .HasColumnName("account_id");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("text")
                        .HasColumnName("avatar_url");

                    b.Property<string>("FirstName")
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

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

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.LoginCredentialDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", "Account")
                        .WithOne()
                        .HasForeignKey("TypingMaster.DataAccess.Dao.LoginCredentialDao", "AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.LoginLogDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TypingMaster.DataAccess.Dao.PracticeLogDao", b =>
                {
                    b.HasOne("TypingMaster.DataAccess.Dao.AccountDao", "Account")
                        .WithOne("History")
                        .HasForeignKey("TypingMaster.DataAccess.Dao.PracticeLogDao", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
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
