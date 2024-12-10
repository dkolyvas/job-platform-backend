using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JobPlatform.Data;

public partial class JobplatformContext : DbContext
{
    public JobplatformContext()
    {
    }

    public JobplatformContext(DbContextOptions<JobplatformContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<ApplicantMerit> ApplicantMerits { get; set; }

    public virtual DbSet<ApplicantSkill> ApplicantSkills { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<SkillCategory> SkillCategories { get; set; }

    public virtual DbSet<SkillLevel> SkillLevels { get; set; }

    public virtual DbSet<SkillSubcategory> SkillSubcategories { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<SubscriptionType> SubscriptionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vacancy> Vacancies { get; set; }

    public virtual DbSet<VacancyMerit> VacancyMerits { get; set; }

    public virtual DbSet<VacancySkill> VacancySkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Greek_100_CI_AI");

        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_APPLICANTS");

            entity.HasIndex(e => e.UserId, "IX_APPLICANTS").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Cv)
                .HasMaxLength(255)
                .HasColumnName("CV");
            entity.Property(e => e.Email)
                .HasMaxLength(70)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("FIRSTNAME");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("LASTNAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("PHONE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.UserId)
                .HasConstraintName("FK_APPLICANTS_USERS");
        });

        modelBuilder.Entity<ApplicantMerit>(entity =>
        {
            entity.ToTable("Applicant_Merits");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicantId).HasColumnName("APPLICANT_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantMerits)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("FK_Applicant_Merits_Applicants");
        });

        modelBuilder.Entity<ApplicantSkill>(entity =>
        {
            entity.ToTable("Applicant_Skills");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicantId).HasColumnName("APPLICANT_ID");
            entity.Property(e => e.DateFrom).HasColumnName("DATE_FROM");
            entity.Property(e => e.DateTo).HasColumnName("DATE_TO");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.DurationMonths).HasColumnName("DURATION_MONTHS");
            entity.Property(e => e.Institution)
                .HasMaxLength(100)
                .HasColumnName("INSTITUTION");
            entity.Property(e => e.SkillLevelId).HasColumnName("SKILL_LEVEL_ID");
            entity.Property(e => e.SkillSubcategoryId).HasColumnName("SKILL_SUBCATEGORY_ID");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applicant_Skills_Applicants1");

            entity.HasOne(d => d.SkillLevel).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.SkillLevelId)
                .HasConstraintName("FK_Applicant_Skills_Skill_Levels");

            entity.HasOne(d => d.SkillSubcategory).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.SkillSubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applicant_Skills_Skill_Subcategories");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicantId).HasColumnName("APPLICANT_ID");
            entity.Property(e => e.ApplicationDate).HasColumnName("APPLICATION_DATE");
            entity.Property(e => e.VacancyId).HasColumnName("VACANCY_ID");
            entity.Property(e => e.Checked).HasColumnName("CHECKED");
            entity.Property(e => e.Approved).HasColumnName("APPROVED");
            entity.Property(e => e.ApplicationText).HasColumnName("APPLICATION_TEXT");

            entity.HasOne(d => d.Applicant).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("FK_Applications_Applicants");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.Applications)
                .HasForeignKey(d => d.VacancyId)
                .HasConstraintName("FK_Applications_Vacancies");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Businesses").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("IMAGE");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("PHONE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.Website)
                .HasMaxLength(70)
                .HasColumnName("WEBSITE");

            entity.HasOne(d => d.User).WithOne(p => p.Business)
                .HasForeignKey<Business>(d => d.UserId)
                .HasConstraintName("FK_Businesses_Users");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<SkillCategory>(entity =>
        {
            entity.ToTable("Skill_Categories");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Checked).HasColumnName("CHECKED");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Sort).HasColumnName("SORT");
        });

        modelBuilder.Entity<SkillLevel>(entity =>
        {
            entity.ToTable("Skill_Levels");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Grade).HasColumnName("GRADE");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("NAME");
            entity.Property(e => e.SkillCategoryId).HasColumnName("SKILL_CATEGORY_ID");
            entity.Property(e => e.SkillSort).HasColumnName("SKILL_SORT");
            entity.Property(e => e.SkillSubcategoryId).HasColumnName("SKILL_SUBCATEGORY_ID");

            entity.HasOne(d => d.SkillSubcategory).WithMany(p => p.SkillLevels)
                .HasForeignKey(d => d.SkillSubcategoryId)
                .HasConstraintName("FK_Skill_Levels_Skill_Subcategories");

            entity.HasOne(d => d.SkillCategory).WithMany(p => p.SkillLevels)
                .HasForeignKey(d => d.SkillCategoryId)
                .HasConstraintName("FK_Skill_Levels_Skill_Categories");
        });

        modelBuilder.Entity<SkillSubcategory>(entity =>
        {
            entity.ToTable("Skill_Subcategories");

            entity.HasIndex(e => e.SkillCategoryId, "IX_Skill_Subcategories").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Checked).HasColumnName("CHECKED");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("NAME");
            entity.Property(e => e.SkillCategoryId).HasColumnName("SKILL_CATEGORY_ID");

            entity.HasOne(d => d.SkillCategory).WithMany(p => p.SkillSubcategories)
                .HasForeignKey(d => d.SkillCategoryId)
                .HasConstraintName("FK_Skill_Subcategories_Skill_Categories");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Allowance).HasColumnName("ALLOWANCE");
            entity.Property(e => e.BusinessId).HasColumnName("BUSINESS_ID");
            entity.Property(e => e.EndDate).HasColumnName("END_DATE");
            entity.Property(e => e.StartDate).HasColumnName("START_DATE");

            entity.HasOne(d => d.Business).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.BusinessId)
                .HasConstraintName("FK_Subscriptions_Businesses");
        });

        modelBuilder.Entity<SubscriptionType>(entity =>
        {
            entity.ToTable("Subscription_Types");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Allowance).HasColumnName("ALLOWANCE");
            entity.Property(e => e.DurationDays).HasColumnName("DURATION_DAYS");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("numeric(8, 2)")
                .HasColumnName("PRICE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("USERNAME");
            entity.Property(e => e.Role)
                .HasMaxLength(12)
                .HasColumnName("ROLE");
            entity.Property(e => e.UnauthorizedCount)
                .HasColumnName("UNAUTHORIZED_COUNT");
            entity.Property(e => e.RestoreCode)
                .HasMaxLength(50)
                .HasColumnName("RESTORE_CODE");
        });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Active).HasColumnName("ACTIVE");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.BusinessId).HasColumnName("BUSINESS_ID");
            entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
            entity.Property(e => e.PublicationDate).HasColumnName("PUBLICATION_DATE");
            entity.Property(e => e.RegionId).HasColumnName("REGION_ID");
            entity.Property(e => e.SkillSubcategoryId).HasColumnName("SKILL_SUBCATEGORY_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("TITLE");

            entity.HasOne(d => d.Business).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.BusinessId)
                .HasConstraintName("FK_Vacancies_Businesses");

            entity.HasOne(d => d.Region).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacancies_Regions");

            entity.HasOne(d => d.SkillSubcategory).WithMany(p => p.Vacancies)
                .HasForeignKey(d => d.SkillSubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacancies_Skill_Subcategories");
        });

        modelBuilder.Entity<VacancyMerit>(entity =>
        {
            entity.ToTable("Vacancy_Merits");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .HasColumnName("NAME");
            entity.Property(e => e.VacancyId).HasColumnName("VACANCY_ID");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.VacancyMerits)
                .HasForeignKey(d => d.VacancyId)
                .HasConstraintName("FK_Vacancy_Merits_Vacancies");
        });

        modelBuilder.Entity<VacancySkill>(entity =>
        {
            entity.ToTable("Vacancy_Skills");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Duration).HasColumnName("DURATION");
          
            entity.Property(e => e.Required).HasColumnName("REQUIRED");
            entity.Property(e => e.SkillCategoryId).HasColumnName("SKILL_CATEGORY_ID");
            entity.Property(e => e.SkillLevelId).HasColumnName("SKILL_LEVEL_ID");
            entity.Property(e => e.SkillSort).HasColumnName("SKILL_SORT");
            entity.Property(e => e.SkillSubcategoryId).HasColumnName("SKILL_SUBCATEGORY_ID");
            entity.Property(e => e.VacancyId).HasColumnName("VACANCY_ID");

            entity.HasOne(d => d.SkillCategory).WithMany(p => p.VacancySkills)
                .HasForeignKey(d => d.SkillCategoryId)
                .HasConstraintName("FK_Vacancy_Skills_Skill_Categories");

            entity.HasOne(d => d.SkillLevel).WithMany(p => p.VacancySkills)
                .HasForeignKey(d => d.SkillLevelId)
                .HasConstraintName("FK_Vacancy_Skills_Skill_Levels");

            entity.HasOne(d => d.SkillSubcategory).WithMany(p => p.VacancySkills)
                .HasForeignKey(d => d.SkillSubcategoryId)
                .HasConstraintName("FK_Vacancy_Skills_Skill_Subcategories");

            entity.HasOne(d => d.Vacancy).WithMany(p => p.VacancySkills)
                .HasForeignKey(d => d.VacancyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacancy_Skills_Vacancies");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
