using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface ICvService
{
    Task<PersonalDataDto> AddPersonalDataAsync(PersonalDataDto dto, Guid userId);
    Task<PersonalDataDto> UpdatePersonalDataAsync(int id, PersonalDataDto dto, Guid userId);
    Task DeletePersonalDataAsync(int id, Guid userId);
    Task AddWorkExperienceAsync(WorkExperienceDto dto, Guid userId);
    Task UpdateWorkExperienceAsync(int id, WorkExperienceDto dto, Guid userId);
    Task RemoveWorkExperienceAsync(int id, Guid userId);

    Task AddEducationAsync(EducationDto dto, Guid userId);
    Task UpdateEducationAsync(int id, EducationDto dto, Guid userId);
    Task RemoveEducationAsync(int id, Guid userId);

    Task AddLanguageAsync(LanguageDto dto, Guid userId);
    Task UpdateLanguageAsync(int id, LanguageDto dto, Guid userId);
    Task RemoveLanguageAsync(int id, Guid userId);
    Task<GeneratedCV> GenerateCvAsync(Guid userId);

}