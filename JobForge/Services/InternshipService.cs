using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace JobForge.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly AppDbContext _context;

        public InternshipService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IntershipDto> CreateInternshipAsync(IntershipDto internshipDto, Guid authorId)
        {
            var internship = new Internship
            {
                Id = Guid.NewGuid(),
                Title = internshipDto.Title,
                Description = internshipDto.Description,
                StartDate = internshipDto.StartDate,
                EndDate = internshipDto.EndDate,
                AuthorId = authorId,       
                Salary = internshipDto.Salary
            };

            _context.Internships.Add(internship);
            await _context.SaveChangesAsync();

            internshipDto = new IntershipDto
            {
                Title = internship.Title,
                Description = internship.Description,
                StartDate = internship.StartDate,
                EndDate = internship.EndDate,
                Salary = internship.Salary
            };

            return internshipDto;
        }

        public async Task<IntershipDto?> GetInternshipByIdAsync(Guid id)
        {
            var entity = await _context.Internships.FindAsync(id);
            if (entity == null) return null;

            return new IntershipDto
            {
                Title = entity.Title,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Salary = entity.Salary
            };
        }

        public async Task<List<IntershipDto>> GetAllInternshipsAsync()
        {
            return await _context.Internships
                .Select(i => new IntershipDto
                {
                    Title = i.Title,
                    Description = i.Description,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    Salary = i.Salary
                })
                .ToListAsync();
        }

        
        public async Task<bool> DeleteInternshipAsync(Guid internshipId)
        {
            var internship = await _context.Internships.FindAsync(internshipId);
            if (internship == null)
                return false;

            _context.Internships.Remove(internship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InternshipApplicationDto> CreateApplicationAsync(InternshipApplicationDto applicationDto, Guid userId)
        {
            var internshipExists = await _context.Internships.AnyAsync(i => i.Id == applicationDto.InternshipId);
            if (!internshipExists)
            {
                throw new KeyNotFoundException($"Internship with ID '{applicationDto.InternshipId}' not found.");
            }

            var application = new InternshipApplication
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                InternshipId = applicationDto.InternshipId,
                AppliedAt = DateTime.UtcNow
            };

            _context.InternshipApplications.Add(application);
            await _context.SaveChangesAsync();

            return new InternshipApplicationDto
            {
                InternshipId = application.InternshipId
            };
        }

        public async Task<List<InternshipApplicationDto>> GetApplicationsByInternshipIdAsync(Guid internshipId)
        {
            var internshipExists = await _context.Internships.AnyAsync(i => i.Id == internshipId);
            if (!internshipExists)
                throw new KeyNotFoundException($"Internship with ID '{internshipId}' not found.");

            return await _context.InternshipApplications
                .Where(app => app.InternshipId == internshipId)
                .Select(app => new InternshipApplicationDto
                {
                    InternshipId = app.InternshipId,
                })
                .ToListAsync();
        }
        
        public async Task<IEnumerable<InternshipApplication>> GetApplicationsAsync(Guid? internshipId = null)
        {
            var query = _context.InternshipApplications.AsQueryable();

            if (internshipId.HasValue)
            {
                query = query.Where(a => a.InternshipId == internshipId.Value);
            }

            return await query.ToListAsync();
        }




        public async Task<bool> DeleteApplicationAsync(Guid applicationId)
        {
            var application = await _context.InternshipApplications.FindAsync(applicationId);
            if (application == null)
                return false;

            _context.InternshipApplications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
