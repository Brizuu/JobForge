using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface ICvService
{
    Task AddPersonalInformations(Guid userId, PersonalInformationDto dto);
    Task<PersonalInformationDto?> GetPersonalInformations(Guid userId);
    Task UpdatePersonalInformations(Guid userId, PersonalInformationEditDto dto);
    
    
    Task AddWorkExperience(Guid userId, WorkExperienceDto dto);
    Task<IEnumerable<WorkExperienceEditDto>> GetWorkExperienceAsync(Guid userId);
    Task<bool> UpdateWorkExperience(Guid userId, Guid workExperienceId, WorkExperienceEditDto dto);
    Task DeleteWorkExperience(Guid userId, Guid id);
    
    
    Task AddLanguage(Guid userId, LanguageDto dto);
    Task<List<LanguageEditDto>> GetUserLanguages(Guid userId);
    Task<bool> UpdateLanguage(Guid userId, Guid languageId, LanguageEditDto dto);
    Task DeleteLanguage(Guid userId, Guid id);

    
    Task<SoftSkills> AddSoftSkills(Guid userId, SoftSkillsDto dto);
    Task<List<SoftSkillsEditDto>> GetSoftSkills(Guid userId);
    Task<bool> UpdateSoftSkills(Guid userId, Guid skillId, SoftSkillsEditDto dto);
    Task DeleteSoftSkills(Guid userId, Guid id);
    
    
    Task<TechnicalSkills> AddTechnicalSkill(Guid userId, TechnicalSkillsDto dto);
    Task<List<TechnicalSkillsEditDto>> GetTechnicalSkills(Guid userId);
    Task<bool> UpdateTechnicalSkill(Guid userId, Guid skillId, TechnicalSkillsEditDto dto);
    Task DeleteTechnicalSkill(Guid userId, Guid id);
    
    
    Task<Interests> AddInterest(Guid userId, InterestsDto dto);
    Task<List<InterestsEditDto>> GetInterests(Guid userId);
    Task<bool> UpdateInterest(Guid userId, Guid interestId, InterestsEditDto dto);
    Task DeleteInterest(Guid userId, Guid interestId);
    
    
    Task AddUserCourse(Guid userId, UserCourseDto dto);
    Task<List<UserCourseEditDto>> GetUserCourses(Guid userId);
    Task UpdateUserCourse(Guid userId, Guid courseId, UserCourseEditDto dto);
    Task DeleteUserCourse(Guid userId, Guid id);
    
    
    Task<GeneratedCV> GenerateCvAsync(Guid userId);
    Task<GeneratedCVDto?> GetGeneratedCvAsync(Guid userId);
}