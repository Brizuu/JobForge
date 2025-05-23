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

    // public async Task SavePersonalInformationAsync(PersonalInformationDto dto, Guid userId)
    // {
    //     var entity = new PersonalInformation
    //     {
    //         UserId = userId, 
    //         FirstName = dto.FirstName,
    //         LastName = dto.LastName,
    //         PhoneNumber = dto.PhoneNumber,
    //         EmailAddress = dto.EmailAddress,
    //         LinkedinUrl = dto.LinkedinUrl,
    //         Summary = dto.Summary,
    //         TechnicalSkills = dto.TechnicalSkills ?? new(),
    //         SoftSkills = dto.SoftSkills ?? new(),
    //         Interests = dto.Interests ?? new(),
    //         Certificates = dto.Certificates ?? new(),
    //         Courses = dto.Courses ?? new(),
    //         
    //         WorkExperiences = dto.WorkExperiences?.Select(w => new WorkExperience
    //         {
    //             UserId = userId,
    //             CompanyName = w.CompanyName,
    //             EmploymentDateStart = w.EmploymentDateStart.ToUniversalTime(),
    //             EmploymentDateEnd = w.EmploymentDateEnd.ToUniversalTime(),
    //             Responsibilities = w.Responsibilities
    //         }).ToList() ?? new(),
    //
    //         Educations = dto.Educations?.Select(e => new Education
    //         {
    //             UserId = userId,  
    //             SchoolName = e.SchoolName,
    //             Major = e.Major,
    //             EducationDateStart = e.EducationDateStart.ToUniversalTime(),
    //             EducationDateEnd = e.EducationDateEnd.ToUniversalTime()
    //         }).ToList() ?? new(),
    //
    //         Languages = dto.Languages?.Select(l => new Language
    //         {
    //             UserId = userId, 
    //             LanguageName = l.LanguageName,
    //             ProficiencyLevel = l.ProficiencyLevel
    //         }).ToList() ?? new()
    //     };
    //
    //     _context.PersonalInformations.Add(entity);
    //     await _context.SaveChangesAsync();
    // }
    
   public async Task<PersonalDataDto> AddPersonalDataAsync(PersonalDataDto dto, Guid userId)
    {
        var entity = new PersonalData
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailAddress = dto.EmailAddress,
            LinkedinUrl = dto.LinkedinUrl,
            Summary = dto.Summary,
            TechnicalSkills = dto.TechnicalSkills,
            SoftSkills = dto.SoftSkills,
            Interests = dto.Interests,
            Certificates = dto.Certificates,
            Courses = dto.Courses
        };

        _context.PersonalData.Add(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        dto.UserId = userId;

        return dto;
    }

    public async Task<PersonalDataDto> UpdatePersonalDataAsync(int id, PersonalDataDto dto, Guid userId)
    {
        var entity = await _context.PersonalData.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (entity == null)
            throw new InvalidOperationException("Dane osobowe nie znalezione lub brak dostępu.");

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.PhoneNumber = dto.PhoneNumber;
        entity.EmailAddress = dto.EmailAddress;
        entity.LinkedinUrl = dto.LinkedinUrl;
        entity.Summary = dto.Summary;
        entity.TechnicalSkills = dto.TechnicalSkills;
        entity.SoftSkills = dto.SoftSkills;
        entity.Interests = dto.Interests;
        entity.Certificates = dto.Certificates;
        entity.Courses = dto.Courses;

        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        dto.UserId = entity.UserId;

        return dto;
    }

    public async Task DeletePersonalDataAsync(int id, Guid userId)
    {
        var entity = await _context.PersonalData.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (entity == null)
            throw new InvalidOperationException("Dane osobowe nie znalezione lub brak dostępu.");

        _context.PersonalData.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> UserIsPremium(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Premium");
    }

    
    public async Task<GeneratedCV> GenerateCvAsync(Guid userId)
    {
        var personalInfo = await _context.PersonalInformations
            .Include(p => p.WorkExperiences)
            .Include(p => p.Educations)
            .Include(p => p.Languages)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (personalInfo == null)
            throw new InvalidOperationException("Brak danych osobowych. Uzupełnij profil przed wygenerowaniem CV.");

        if (!await UserIsPremium(userId))
            throw new UnauthorizedAccessException("Tylko użytkownicy Premium mogą generować CV.");

        
        var result = new
        {
            PersonalInformation = personalInfo,
            WorkExperience = personalInfo.WorkExperiences,
            Education = personalInfo.Educations,
            Languages = personalInfo.Languages
        };

        var contentJson = JsonSerializer.Serialize(result);

        var cv = new GeneratedCV
        {
            UserId = userId,
            ContentJson = contentJson
        };

        _context.GeneratedCVs.Add(cv);
        await _context.SaveChangesAsync();

        return cv;
    }

    
    // -------------------- WorkExperience --------------------

    public async Task AddWorkExperienceAsync(WorkExperienceDto dto, Guid userId)
    {
        // Sprawdzenie, czy istnieje PersonalInformation dla tego użytkownika
        var personalInformation = await _context.PersonalInformations
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (personalInformation == null)
        {
            throw new InvalidOperationException("Nie znaleziono danych osobowych dla tego użytkownika.");
        }

        // Tworzenie nowego doświadczenia zawodowego
        var experience = new WorkExperience
        {
            UserId = userId,
            CompanyName = dto.CompanyName,
            EmploymentDateStart = dto.EmploymentDateStart.ToUniversalTime(),
            EmploymentDateEnd = dto.EmploymentDateEnd.ToUniversalTime(),
            Responsibilities = dto.Responsibilities,
            PersonalInformationId = personalInformation.Id // Przypisanie odpowiedniego PersonalInformationId
        };

        // Dodanie doświadczenia zawodowego do bazy
        _context.WorkExperiences.Add(experience);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateWorkExperienceAsync(int id, WorkExperienceDto dto, Guid userId)
    {
        var workExperience = await _context.WorkExperiences
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workExperience == null)
        {
            throw new InvalidOperationException("Nie znaleziono doświadczenia zawodowego.");
        }

        // Aktualizacja danych doświadczenia
        workExperience.CompanyName = dto.CompanyName;
        workExperience.EmploymentDateStart = dto.EmploymentDateStart.ToUniversalTime();
        workExperience.EmploymentDateEnd = dto.EmploymentDateEnd.ToUniversalTime();
        workExperience.Responsibilities = dto.Responsibilities;

        await _context.SaveChangesAsync();
    }



    public async Task RemoveWorkExperienceAsync(int id, Guid userId)
    {
        var entity = await _context.WorkExperiences
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entity != null)
        {
            _context.WorkExperiences.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    // -------------------- Education --------------------

    public async Task AddEducationAsync(EducationDto dto, Guid userId)
    {
        // Sprawdzenie, czy istnieje PersonalInformation dla tego użytkownika
        var personalInformation = await _context.PersonalInformations
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (personalInformation == null)
        {
            throw new InvalidOperationException("Nie znaleziono danych osobowych dla tego użytkownika.");
        }

        // Tworzenie nowego wpisu edukacji
        var education = new Education
        {
            UserId = userId,
            SchoolName = dto.SchoolName,
            Major = dto.Major,
            EducationDateStart = dto.EducationDateStart.ToUniversalTime(),
            EducationDateEnd = dto.EducationDateEnd.ToUniversalTime(),
            PersonalInformationId = personalInformation.Id // Przypisanie odpowiedniego PersonalInformationId
        };

        // Dodanie edukacji do bazy
        _context.Educations.Add(education);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateEducationAsync(int id, EducationDto dto, Guid userId)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (education == null)
        {
            throw new InvalidOperationException("Nie znaleziono edukacji.");
        }

        // Aktualizacja danych edukacji
        education.SchoolName = dto.SchoolName;
        education.Major = dto.Major;
        education.EducationDateStart = dto.EducationDateStart.ToUniversalTime();
        education.EducationDateEnd = dto.EducationDateEnd.ToUniversalTime();

        await _context.SaveChangesAsync();
    }

    public async Task RemoveEducationAsync(int id, Guid userId)
    {
        var entity = await _context.Educations
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entity != null)
        {
            _context.Educations.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    // -------------------- Language --------------------

    public async Task AddLanguageAsync(LanguageDto dto, Guid userId)
    {
        // Sprawdzenie, czy istnieje PersonalInformation dla tego użytkownika
        var personalInformation = await _context.PersonalInformations
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (personalInformation == null)
        {
            throw new InvalidOperationException("Nie znaleziono danych osobowych dla tego użytkownika.");
        }

        // Tworzenie nowego wpisu językowego
        var language = new Language
        {
            UserId = userId,
            LanguageName = dto.LanguageName,
            ProficiencyLevel = dto.ProficiencyLevel,
            PersonalInformationId = personalInformation.Id // Przypisanie odpowiedniego PersonalInformationId
        };

        // Dodanie języka do bazy
        _context.Languages.Add(language);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateLanguageAsync(int id, LanguageDto dto, Guid userId)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

        if (language == null)
        {
            throw new InvalidOperationException("Nie znaleziono języka.");
        }

        // Aktualizacja danych języka
        language.LanguageName = dto.LanguageName;
        language.ProficiencyLevel = dto.ProficiencyLevel;

        await _context.SaveChangesAsync();
    }

    public async Task RemoveLanguageAsync(int id, Guid userId)
    {
        var entity = await _context.Languages
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        if (entity != null)
        {
            _context.Languages.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
