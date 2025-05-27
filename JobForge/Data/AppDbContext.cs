using JobForge.DbModels;
using JobForge.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<PersonalInformation> PersonalInformations { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Language> Languages { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<JobOffer> JobOffers { get; set; }
    public DbSet<JobOfferTechnology> JobOfferTechnologies { get; set; }
    
    public DbSet<GeneratedCV> GeneratedCVs { get; set; }
    
    public DbSet<PersonalData> PersonalData { get; set; }
    
    public DbSet<JobApplication> JobApplications { get; set; }

    public DbSet<FavoriteJobOffer> FavoriteJobOffers { get; set; }
    
    public DbSet<EmploymentContract> EmploymentContracts { get; set; }
    
    public DbSet<CourseDto> Courses { get; set; }
    public DbSet<CourseSectionDto> CourseSections { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FavoriteJobOffer>()
            .HasKey(f => new { f.UserId, f.JobOfferId });

        modelBuilder.Entity<CourseDto>()
            .HasMany(c => c.Sections)
            .WithOne()
            .HasForeignKey(s => s.CourseId);
        
        modelBuilder.Entity<PersonalInformation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TechnicalSkills)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            entity.Property(e => e.SoftSkills)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            entity.Property(e => e.Interests)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            entity.Property(e => e.Certificates)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            entity.Property(e => e.Courses)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

            entity.HasMany(e => e.WorkExperiences)
                .WithOne(w => w.PersonalInformation)
                .HasForeignKey(w => w.PersonalInformationId);

            entity.HasMany(e => e.Educations)
                .WithOne(ed => ed.PersonalInformation)
                .HasForeignKey(ed => ed.PersonalInformationId);

            entity.HasMany(e => e.Languages)
                .WithOne(l => l.PersonalInformation)
                .HasForeignKey(l => l.PersonalInformationId);
        });
    }
}