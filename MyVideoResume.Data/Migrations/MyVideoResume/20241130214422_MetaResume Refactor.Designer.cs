﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyVideoResume.Data;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    [DbContext(typeof(DataContext))]
    [Migration("20241130214422_MetaResume Refactor")]
    partial class MetaResumeRefactor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyVideoResume.Data.Models.JobPreferencesEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.PrimitiveCollection<string>("EmploymentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Industry")
                        .HasColumnType("int");

                    b.Property<float>("MinimumSalary")
                        .HasColumnType("real");

                    b.Property<int>("PaySchedule")
                        .HasColumnType("int");

                    b.Property<int>("Seniority")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JobPreferences", t =>
                        {
                            t.HasTrigger("JobPreferences_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.Resume.MetaResumeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BasicsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("UserAccountEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BasicsId");

                    b.HasIndex("UserAccountEntityId");

                    b.ToTable("MetaResumes", t =>
                        {
                            t.HasTrigger("MetaResumes_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.UserAccountEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("JobPreferencesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("JobPreferencesId");

                    b.ToTable("UserAccounts", t =>
                        {
                            t.HasTrigger("UserAccounts_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Award", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Awarder")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Award", t =>
                        {
                            t.HasTrigger("Award_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Basics", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Basics", t =>
                        {
                            t.HasTrigger("Basics_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Certificate", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Certificate", t =>
                        {
                            t.HasTrigger("Certificate_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Education", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Area")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Courses")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Institution")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Score")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Education", t =>
                        {
                            t.HasTrigger("Education_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Interest", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.PrimitiveCollection<string>("Keywords")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Interest", t =>
                        {
                            t.HasTrigger("Interest_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.LanguageItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Fluency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("LanguageItem", t =>
                        {
                            t.HasTrigger("LanguageItem_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Location", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Location", t =>
                        {
                            t.HasTrigger("Location_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Highlights")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Project", t =>
                        {
                            t.HasTrigger("Project_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Publication", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Publisher")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReleaseDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Publication", t =>
                        {
                            t.HasTrigger("Publication_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.ReferenceItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("ReferenceItem", t =>
                        {
                            t.HasTrigger("ReferenceItem_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Skill", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.PrimitiveCollection<string>("Keywords")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Skill", t =>
                        {
                            t.HasTrigger("Skill_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Volunteer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Highlights")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Organization")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Volunteer", t =>
                        {
                            t.HasTrigger("Volunteer_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Work", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.PrimitiveCollection<string>("Highlights")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MetaResumeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MetaResumeEntityId");

                    b.ToTable("Work", t =>
                        {
                            t.HasTrigger("Work_Trigger");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.Resume.MetaResumeEntity", b =>
                {
                    b.HasOne("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Basics", "Basics")
                        .WithMany()
                        .HasForeignKey("BasicsId");

                    b.HasOne("MyVideoResume.Data.Models.UserAccountEntity", null)
                        .WithMany("Resumes")
                        .HasForeignKey("UserAccountEntityId");

                    b.Navigation("Basics");
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.UserAccountEntity", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.JobPreferencesEntity", "JobPreferences")
                        .WithMany()
                        .HasForeignKey("JobPreferencesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JobPreferences");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Award", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Awards")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Basics", b =>
                {
                    b.HasOne("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Certificate", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Certificates")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Education", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Education")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Interest", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Interests")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.LanguageItem", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Languages")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Project", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Projects")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Publication", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Publications")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.ReferenceItem", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("References")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Skill", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Skills")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Volunteer", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Volunteer")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.ResumeAbstractions.Formats.JSONResumeFormat.Work", b =>
                {
                    b.HasOne("MyVideoResume.Data.Models.Resume.MetaResumeEntity", null)
                        .WithMany("Work")
                        .HasForeignKey("MetaResumeEntityId");
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.Resume.MetaResumeEntity", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Certificates");

                    b.Navigation("Education");

                    b.Navigation("Interests");

                    b.Navigation("Languages");

                    b.Navigation("Projects");

                    b.Navigation("Publications");

                    b.Navigation("References");

                    b.Navigation("Skills");

                    b.Navigation("Volunteer");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("MyVideoResume.Data.Models.UserAccountEntity", b =>
                {
                    b.Navigation("Resumes");
                });
#pragma warning restore 612, 618
        }
    }
}
