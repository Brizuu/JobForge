using System.Text.Json;
using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class CvService : ICvService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CvService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    ////////////////////////////////// Personal informations ////////////////////////////////
    
    public async Task AddPersonalInformations(Guid userId, PersonalInformationDto dto)
    {
        var exists = await _context.PersonalInformations.AnyAsync(p => p.UserId == userId);
        if (exists) throw new InvalidOperationException("Personal info already exists.");

        var entity = new PersonalInformation
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailAddress = dto.EmailAddress,
            PhoneNumber = dto.PhoneNumber,
            LinkedinUrl = dto.LinkedinUrl,
            Summary = dto.Summary
        };

        _context.PersonalInformations.Add(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<PersonalInformationDto?> GetPersonalInformations(Guid userId)
    {
        var entity = await _context.PersonalInformations
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return entity == null ? null : new PersonalInformationDto
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            EmailAddress = entity.EmailAddress,
            PhoneNumber = entity.PhoneNumber,
            LinkedinUrl = entity.LinkedinUrl,
            Summary = entity.Summary
        };
    }
    
    public async Task UpdatePersonalInformations(Guid userId, PersonalInformationEditDto dto)
    {
        var entity = await _context.PersonalInformations
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (entity == null) throw new KeyNotFoundException("Personal info not found.");

        entity.FirstName = dto.FirstName ?? entity.FirstName;
        entity.LastName = dto.LastName ?? entity.LastName;
        entity.EmailAddress = dto.EmailAddress ?? entity.EmailAddress;
        entity.PhoneNumber = dto.PhoneNumber ?? entity.PhoneNumber;
        entity.LinkedinUrl = dto.LinkedinUrl ?? entity.LinkedinUrl;
        entity.Summary = dto.Summary ?? entity.Summary;

        await _context.SaveChangesAsync();
    }
    
    ////////////////////////////////// Work Experience ////////////////////////////////
    
    public async Task AddWorkExperience(Guid userId, WorkExperienceDto dto)
    {
        var exists = await _context.WorkExperiences.AnyAsync(x =>
            x.UserId == userId &&
            x.CompanyName == dto.CompanyName &&
            x.PositionTitle == dto.PositionTitle);

        if (exists)
            throw new InvalidOperationException("To doświadczenie już istnieje.");

        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var entity = new WorkExperience
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GeneratedCVId = generatedCv.Id,
            CompanyName = dto.CompanyName,
            PositionTitle = dto.PositionTitle,
            Location = dto.Location,
            EmploymentType = dto.EmploymentType,
            EmploymentDateStart = dto.EmploymentDateStart,
            EmploymentDateEnd = dto.EmploymentDateEnd,
            Responsibilities = dto.Responsibilities,
            TechnologiesUsed = dto.TechnologiesUsed
        };

        _context.WorkExperiences.Add(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<WorkExperienceEditDto>> GetWorkExperienceAsync(Guid userId)
    {
        return await _context.WorkExperiences
            .Where(x => x.UserId == userId)
            .Select(x => new WorkExperienceEditDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                PositionTitle = x.PositionTitle,
                Location = x.Location,
                EmploymentType = x.EmploymentType,
                EmploymentDateStart = x.EmploymentDateStart,
                EmploymentDateEnd = x.EmploymentDateEnd,
                Responsibilities = x.Responsibilities,
                TechnologiesUsed = x.TechnologiesUsed
            }).ToListAsync();
    }

    
    public async Task<bool> UpdateWorkExperience(Guid userId, Guid workExperienceId, WorkExperienceEditDto dto)
    {
        var workExp = await _context.WorkExperiences
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == workExperienceId);

        if (workExp == null)
            return false;

        if (!string.IsNullOrEmpty(dto.CompanyName))
            workExp.CompanyName = dto.CompanyName;

        if (!string.IsNullOrEmpty(dto.PositionTitle))
            workExp.PositionTitle = dto.PositionTitle;

        if (!string.IsNullOrEmpty(dto.Location))
            workExp.Location = dto.Location;

        if (!string.IsNullOrEmpty(dto.EmploymentType))
            workExp.EmploymentType = dto.EmploymentType;

        if (dto.EmploymentDateStart.HasValue)
            workExp.EmploymentDateStart = dto.EmploymentDateStart.Value;

        if (dto.EmploymentDateEnd.HasValue)
            workExp.EmploymentDateEnd = dto.EmploymentDateEnd.Value;

        if (!string.IsNullOrEmpty(dto.Responsibilities))
            workExp.Responsibilities = dto.Responsibilities;

        if (!string.IsNullOrEmpty(dto.TechnologiesUsed))
            workExp.TechnologiesUsed = dto.TechnologiesUsed;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteWorkExperience(Guid userId, Guid workExperienceId)
    {
        var workExp = await _context.WorkExperiences
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == workExperienceId);

        if (workExp == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.WorkExperiences.Remove(workExp);
        await _context.SaveChangesAsync();
    }
    
    ////////////////////////////////// Languages ////////////////////////////////
    
    public async Task AddLanguage(Guid userId, LanguageDto dto)
    {
        var exists = await _context.Languages.AnyAsync(x =>
            x.UserId == userId && x.LanguageName.ToLower() == dto.LanguageName.ToLower());

        if (exists)
            throw new InvalidOperationException("Język już istnieje.");

        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var language = new Language
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GeneratedCVId = generatedCv.Id,
            LanguageName = dto.LanguageName,
            ProficiencyLevel = dto.ProficiencyLevel,
            AdditionalDescription = dto.AdditionalDescription
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LanguageEditDto>> GetUserLanguages(Guid userId)
    {
        return await _context.Languages
            .Where(x => x.UserId == userId)
            .Select(x => new LanguageEditDto
            {
                Id = x.Id,
                LanguageName = x.LanguageName,
                ProficiencyLevel = x.ProficiencyLevel,
                AdditionalDescription = x.AdditionalDescription
            })
            .ToListAsync();
    }



    public async Task<bool> UpdateLanguage(Guid userId, Guid languageId, LanguageEditDto dto)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == languageId);

        if (language == null)
            return false;

        if (dto.ProficiencyLevel.HasValue)
            language.ProficiencyLevel = dto.ProficiencyLevel.Value;

        if (dto.AdditionalDescription != null)
            language.AdditionalDescription = dto.AdditionalDescription;

        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task DeleteLanguage(Guid userId, Guid languageId)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == languageId);

        if (language == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();
    }

    ////////////////////////////////// Soft Skills ////////////////////////////////
    
    public async Task<SoftSkills> AddSoftSkills(Guid userId, SoftSkillsDto dto)
    {
        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var newSkill = new SoftSkills
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GeneratedCVId = generatedCv.Id,
            SkillName = dto.SkillName,
            ProficiencyLevel = dto.ProficiencyLevel,
            AdditionalDescription = dto.AdditionalDescription
        };

        _context.SoftSkills.Add(newSkill);
        await _context.SaveChangesAsync();

        return newSkill;
    }
    
    public async Task<List<SoftSkillsEditDto>> GetSoftSkills(Guid userId)
    {
        return await _context.SoftSkills
            .Where(s => s.UserId == userId)
            .Select(s => new SoftSkillsEditDto
            {
                Id = s.Id,
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel,
                AdditionalDescription = s.AdditionalDescription
            })
            .ToListAsync();
    }


    public async Task<bool> UpdateSoftSkills(Guid userId, Guid skillId, SoftSkillsEditDto dto)
    {
        var skill = await _context.SoftSkills
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == skillId);

        if (skill == null)
            return false;

        if (!string.IsNullOrEmpty(dto.SkillName))
            skill.SkillName = dto.SkillName;

        if (dto.ProficiencyLevel.HasValue)
            skill.ProficiencyLevel = dto.ProficiencyLevel;

        if (!string.IsNullOrEmpty(dto.AdditionalDescription))
            skill.AdditionalDescription = dto.AdditionalDescription;

        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task DeleteSoftSkills(Guid userId, Guid skillId)
    {
        var skill = await _context.SoftSkills
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == skillId);

        if (skill == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.SoftSkills.Remove(skill);
        await _context.SaveChangesAsync();
    }
    
    ////////////////////////////////// Technical Skills ////////////////////////////////
    
    public async Task<TechnicalSkills> AddTechnicalSkill(Guid userId, TechnicalSkillsDto dto)
    {
        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var newSkill = new TechnicalSkills
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GeneratedCVId = generatedCv.Id,
            SkillName = dto.SkillName,
            ProficiencyLevel = dto.ProficiencyLevel,
            AdditionalDescription = dto.AdditionalDescription
        };

        _context.TechnicalSkills.Add(newSkill);
        await _context.SaveChangesAsync();

        return newSkill;
    }

    public async Task<List<TechnicalSkillsEditDto>> GetTechnicalSkills(Guid userId)
    {
        return await _context.TechnicalSkills
            .Where(s => s.UserId == userId)
            .Select(s => new TechnicalSkillsEditDto
            {
                Id = s.Id,
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel,
                AdditionalDescription = s.AdditionalDescription
            })
            .ToListAsync();
    }


    public async Task<bool> UpdateTechnicalSkill(Guid userId, Guid skillId, TechnicalSkillsEditDto dto)
    {
        var skill = await _context.TechnicalSkills
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == skillId);

        if (skill == null)
            return false;

        if (!string.IsNullOrEmpty(dto.SkillName))
            skill.SkillName = dto.SkillName;

        if (dto.ProficiencyLevel.HasValue)
            skill.ProficiencyLevel = dto.ProficiencyLevel.Value;

        if (!string.IsNullOrEmpty(dto.AdditionalDescription))
            skill.AdditionalDescription = dto.AdditionalDescription;

        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task DeleteTechnicalSkill(Guid userId, Guid skillId)
    {
        var skill = await _context.TechnicalSkills
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == skillId);

        if (skill == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.TechnicalSkills.Remove(skill);
        await _context.SaveChangesAsync();
    }
    
    ////////////////////////////////// Interests ////////////////////////////////
    
    public async Task<Interests> AddInterest(Guid userId, InterestsDto dto)
    {
        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var newInterest = new Interests
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GeneratedCVId = generatedCv.Id,
            InterestName = dto.InterestName,
            ProficiencyLevel = dto.ProficiencyLevel,
            AdditionalDescription = dto.AdditionalDescription
        };

        _context.Interests.Add(newInterest);
        await _context.SaveChangesAsync();

        return newInterest;
    }

    public async Task<List<InterestsEditDto>> GetInterests(Guid userId)
    {
        return await _context.Interests
            .Where(i => i.UserId == userId)
            .Select(i => new InterestsEditDto
            {
                Id = i.Id,
                InterestName = i.InterestName,
                ProficiencyLevel = i.ProficiencyLevel,
                AdditionalDescription = i.AdditionalDescription
            })
            .ToListAsync();
    }


    public async Task<bool> UpdateInterest(Guid userId, Guid interestId, InterestsEditDto dto)
    {
        var interest = await _context.Interests
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Id == interestId);

        if (interest == null)
            return false;

        if (!string.IsNullOrEmpty(dto.InterestName))
            interest.InterestName = dto.InterestName;

        if (dto.ProficiencyLevel.HasValue)
            interest.ProficiencyLevel = dto.ProficiencyLevel;

        if (!string.IsNullOrEmpty(dto.AdditionalDescription))
            interest.AdditionalDescription = dto.AdditionalDescription;

        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task DeleteInterest(Guid userId, Guid interestId)
    {
        var interest = await _context.Interests
            .FirstOrDefaultAsync(i => i.UserId == userId && i.Id == interestId);

        if (interest == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.Interests.Remove(interest);
        await _context.SaveChangesAsync();
    }

    
    ////////////////////////////////// User Courses ////////////////////////////////
    
    public async Task AddUserCourse(Guid userId, UserCourseDto dto)
    {
        var generatedCv = await _context.GeneratedCVs
            .AsNoTracking()
            .FirstOrDefaultAsync(cv => cv.UserId == userId);
        
        var course = new UserCourse
        {
            UserId = userId,
            Title = dto.Title,
            GeneratedCVId = generatedCv.Id,
            Description = dto.Description,
            Institution = dto.Institution,
            CompletionTime = dto.CompletionTime,
            Category = dto.Category,
            CompletionPercentage = dto.CompletionPercentage
        };

        _context.UserCourses.Add(course);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserCourseEditDto>> GetUserCourses(Guid userId)
    {
        return await _context.UserCourses
            .Where(x => x.UserId == userId)
            .Select(x => new UserCourseEditDto
            {
                Id = x.CourseId,
                Title = x.Title,
                Description = x.Description,
                Institution = x.Institution,
                CompletionTime = x.CompletionTime,
                Category = x.Category,
                CompletionPercentage = x.CompletionPercentage
            })
            .ToListAsync();
    }



    public async Task UpdateUserCourse(Guid userId, Guid courseId, UserCourseEditDto dto)
    {
        var course = await _context.UserCourses
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.UserId == userId);

        if (course == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");
        
        if (!string.IsNullOrWhiteSpace(dto.Title)) 
            course.Title = dto.Title;

        if (!string.IsNullOrWhiteSpace(dto.Description)) 
            course.Description = dto.Description;

        if (!string.IsNullOrWhiteSpace(dto.Institution)) 
            course.Institution = dto.Institution;

        if (dto.CompletionTime.HasValue) 
            course.CompletionTime = dto.CompletionTime;

        if (!string.IsNullOrWhiteSpace(dto.Category)) 
            course.Category = dto.Category;

        if (dto.CompletionPercentage.HasValue) 
            course.CompletionPercentage = dto.CompletionPercentage;

        await _context.SaveChangesAsync();
    }

    
    public async Task DeleteUserCourse(Guid userId, Guid courseId)
    {
        var course = await _context.UserCourses
            .FirstOrDefaultAsync(c => c.CourseId == courseId && c.UserId == userId);

        if (course == null)
            throw new KeyNotFoundException("Course not found or does not belong to the user.");

        _context.UserCourses.Remove(course);
        await _context.SaveChangesAsync();
    }
    
    ////////////////////////////////// CV Generator ////////////////////////////////

    public async Task<GeneratedCV> GenerateCvAsync(Guid userId)
    {
        var personalInfo = await _context.PersonalInformations.FirstOrDefaultAsync(p => p.UserId == userId);
        if (personalInfo == null) 
            throw new KeyNotFoundException("Personal information not found.");

        var educations = await _context.Educations.Where(e => e.UserId == userId).ToListAsync();
        var workExperiences = await _context.WorkExperiences.Where(w => w.UserId == userId).ToListAsync();
        var languages = await _context.Languages.Where(l => l.UserId == userId).ToListAsync();
        var softSkills = await _context.SoftSkills.Where(s => s.UserId == userId).ToListAsync();
        var technicalSkills = await _context.TechnicalSkills.Where(t => t.UserId == userId).ToListAsync();
        var interests = await _context.Interests.Where(i => i.UserId == userId).ToListAsync();
        var userCourses = await _context.UserCourses.Where(c => c.UserId == userId).ToListAsync();

        // Stwórz obiekt GeneratedCV i wypełnij danymi
        var generatedCv = new GeneratedCV
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = personalInfo.FirstName,
            LastName = personalInfo.LastName,
            PhoneNumber = personalInfo.PhoneNumber,
            EmailAddress = personalInfo.EmailAddress ?? string.Empty,
            LinkedinUrl = personalInfo.LinkedinUrl,
            Summary = personalInfo.Summary,
            Educations = educations,
            WorkExperiences = workExperiences,
            Languages = languages,
            SoftSkills = softSkills,
            TechnicalSkills = technicalSkills,
            Interests = interests,
            UserCourse = userCourses,
            GenerationDate = DateTime.UtcNow
        };
        

        // Sprawdź czy CV dla usera już istnieje, usuń jeśli tak
        var existingCv = await _context.GeneratedCVs.FirstOrDefaultAsync(cv => cv.UserId == userId);
        if (existingCv != null)
        {
            _context.GeneratedCVs.Remove(existingCv);
        }

        _context.GeneratedCVs.Add(generatedCv);
        await _context.SaveChangesAsync();

        return generatedCv;
    }

    public async Task<GeneratedCVDto?> GetGeneratedCvAsync(Guid userId)
    {
        var cv = await _context.GeneratedCVs
            .Include(cv => cv.Educations)
            .Include(cv => cv.WorkExperiences)
            .Include(cv => cv.Languages)
            .Include(cv => cv.SoftSkills)
            .Include(cv => cv.TechnicalSkills)
            .Include(cv => cv.Interests)
            .Include(cv => cv.UserCourse)
            .FirstOrDefaultAsync(cv => cv.UserId == userId);

        if (cv == null)
            return null;

        return new GeneratedCVDto
        {
            FirstName = cv.FirstName,
            LastName = cv.LastName,
            PhoneNumber = cv.PhoneNumber,
            EmailAddress = cv.EmailAddress,
            LinkedinUrl = cv.LinkedinUrl,
            Summary = cv.Summary,
            Educations = cv.Educations.Select(e => new EducationDto
            {
                SchoolName = e.SchoolName,
                Specialization = e.Specialization,
                EducationDateStart = e.EducationDateStart,
                EducationDateEnd = e.EducationDateEnd
            }).ToList(),
            WorkExperiences = cv.WorkExperiences.Select(w => new WorkExperienceDto
            {
                CompanyName = w.CompanyName,
                PositionTitle = w.PositionTitle,
                Location = w.Location,
                EmploymentType = w.EmploymentType,
                EmploymentDateStart = w.EmploymentDateStart,
                EmploymentDateEnd = w.EmploymentDateEnd,
                Responsibilities = w.Responsibilities,
                TechnologiesUsed = w.TechnologiesUsed
            }).ToList(),
            Languages = cv.Languages.Select(l => new LanguageDto
            {
                LanguageName = l.LanguageName,
                ProficiencyLevel = l.ProficiencyLevel,
                AdditionalDescription = l.AdditionalDescription
            }).ToList(),
            SoftSkills = cv.SoftSkills.Select(s => new SoftSkillsDto
            {
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel,
                AdditionalDescription = s.AdditionalDescription
            }).ToList(),
            TechnicalSkills = cv.TechnicalSkills.Select(t => new TechnicalSkillsDto
            {
                SkillName = t.SkillName,
                ProficiencyLevel = t.ProficiencyLevel,
                AdditionalDescription = t.AdditionalDescription
            }).ToList(),
            Interests = cv.Interests.Select(i => new InterestsDto
            {
                InterestName = i.InterestName,
                ProficiencyLevel = i.ProficiencyLevel,
                AdditionalDescription = i.AdditionalDescription
            }).ToList(),
            UserCourse = cv.UserCourse.Select(uc => new UserCourseVerifiedDto
            {
                CourseId = uc.CourseId,
                Title = uc.Title,
                Description = uc.Description,
                Institution = uc.Institution,
                CompletionTime = uc.CompletionTime,
                Category = uc.Category,
                isCompleted = uc.isCompleted,
                CompletionPercentage = uc.CompletionPercentage
            }).ToList()

        };
    }

}
