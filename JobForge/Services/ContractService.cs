using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class ContractService : IContractService
{
    private readonly AppDbContext _context;

    public ContractService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddEmploymentContractAsync(Guid userId, EmploymentContractDto dto)
    {
        var contract = new EmploymentContract
        {
            UserId = userId,
            ContractDate = dto.ContractDate,
            EmploymentType = dto.EmploymentType,
            JobTitle = dto.JobTitle,
            CompanyName = dto.CompanyName,
            CompanyAddress = dto.CompanyAddress,
            CompanyNip = dto.CompanyNip,
            WorkplaceLocation = dto.WorkplaceLocation,
            WorkTimeDimension = dto.WorkTimeDimension,
            Salary = dto.Salary,
            AdditionalEmploymentConditions = dto.AdditionalEmploymentConditions,
            WorkStartDate = dto.WorkStartDate.ToUniversalTime(),
            ContractValidFrom = dto.ContractValidFrom?.ToUniversalTime(),
            ContractValidTo = dto.ContractValidTo?.ToUniversalTime(),
            ContractDocumentUrl = dto.ContractDocumentUrl,
            CreatedAt = dto.CreatedAt.ToUniversalTime()
        };

        _context.EmploymentContracts.Add(contract);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> DeleteEmploymentContractAsync(Guid userId, int contractId)
    {
        var contract = await _context.EmploymentContracts
            .FirstOrDefaultAsync(c => c.Id == contractId && c.UserId == userId);

        if (contract == null)
        {
            return false;
        }

        _context.EmploymentContracts.Remove(contract);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UpdateContractStatusAsync(int contractId, Guid contractorId, string status)
    {
        var contract = await _context.EmploymentContracts.FirstOrDefaultAsync(c => c.Id == contractId);

        if (contract == null)
            return false;

        if (contract.Status == "Accepted" || contract.Status == "Rejected")
            return false;

        if (status != "Accepted" && status != "Rejected")
            return false;

        contract.Contractor = contractorId;
        contract.Status = status;

        if (status == "Accepted")
        {
            var personalInfo = await _context.PersonalInformations
                .FirstOrDefaultAsync(pi => pi.UserId == contractorId);

            if (personalInfo == null)
            {
                // Możesz też rzucić wyjątek lub zalogować błąd
                return false;
            }

            var workExperience = new WorkExperience
            {
                PersonalInformationId = personalInfo.Id,  // klucz obcy
                UserId = contractorId,                     // GUID użytkownika
                CompanyName = contract.CompanyName,
                EmploymentDateStart = contract.WorkStartDate,
                EmploymentDateEnd = contract.ContractValidTo ?? DateTime.MaxValue,
                Responsibilities = contract.AdditionalEmploymentConditions ?? string.Empty,
                Verified = "yes"
            };

            _context.WorkExperiences.Add(workExperience);
        }

        await _context.SaveChangesAsync();
        return true;
    }




    public async Task<EmploymentContract?> GetContractByIdAsync(int contractId)
    {
        return await _context.EmploymentContracts.FirstOrDefaultAsync(c => c.Id == contractId);
    }

    public async Task<List<EmploymentContract>> GetUserContractsAsync(Guid userId)
    {
        return await _context.EmploymentContracts
            .Where(c => c.UserId == userId || c.Contractor == userId)
            .ToListAsync();
    }


}