using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Data.Models.Jobs;

namespace MyVideoResume.Data;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        this.OnModelBuilding(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    }
    public DbSet<UserProfileEntity> UserProfiles { get; set; } = default!;
    public DbSet<JobPreferencesEntity> JobPreferences { get; set; } = default!;
    public DbSet<MetaResumeEntity> Resumes { get; set; } = default!;
    public DbSet<MetaDataEntity> MetaData { get; set; } = default!;

    public DbSet<ApplicantToJobEntity> ApplicantsToJobs { get; set; } = default!;

}