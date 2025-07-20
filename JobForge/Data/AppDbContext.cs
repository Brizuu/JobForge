using JobForge.DbModels;
using JobForge.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    ////////////////////////////// CVKI //////////////////////////////////
    
    public DbSet<PersonalInformation> PersonalInformations { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<GeneratedCV> GeneratedCVs { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<SoftSkills> SoftSkills { get; set; }
    public DbSet<TechnicalSkills> TechnicalSkills { get; set; }
    public DbSet<Interests> Interests { get; set; }
    
 
    
    ////////////////////////////// INNE //////////////////////////////////
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<JobOffer> JobOffers { get; set; }
    public DbSet<JobOfferTechnology> JobOfferTechnologies { get; set; }
    
    public DbSet<JobApplication> JobApplications { get; set; }

    public DbSet<FavoriteJobOffer> FavoriteJobOffers { get; set; }
    
    public DbSet<EmploymentContract> EmploymentContracts { get; set; }
    
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseSection> CourseSections { get; set; }
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatUserLink> ChatUserLinks { get; set; }
    
    public DbSet<Internship> Internships { get; set; }
    public DbSet<InternshipApplication> InternshipApplications { get; set; }
    
    public DbSet<InternshipLocation> InternshipLocations { get; set; }
    
    public DbSet<Grant> Grants { get; set; }
    public DbSet<GrantApplication> GrantApplications { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ChatUserLink>()
            .HasIndex(c => new { c.FirstUser, c.SecoundUser })
            .IsUnique();
        
        // modelBuilder.Entity<PersonalInformation>()
        //     .Property(p => p.Courses)
        //     .HasConversion(
        //         v => string.Join(";", v),
        //         v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
        //     );
        
        // modelBuilder.Entity<UserCourse>()
        //     .HasKey(uc => new { uc.UserId, uc.CourseId });

        modelBuilder.Entity<FavoriteJobOffer>()
            .HasKey(f => new { f.UserId, f.JobOfferId });

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Sections)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        ////////////////////////////// CVKI //////////////////////////////////
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.Educations)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.UserCourse)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.SoftSkills)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.WorkExperiences)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.TechnicalSkills)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.Interests)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GeneratedCV>()
            .HasMany(c => c.Languages)
            .WithOne(e => e.CV)
            .HasForeignKey(e => e.GeneratedCVId)
            .OnDelete(DeleteBehavior.Cascade);
       
    }
}