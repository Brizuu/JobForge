using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IContractService
{
    Task AddEmploymentContractAsync(Guid userId, EmploymentContractDto dto);
    Task<bool> DeleteEmploymentContractAsync(Guid userId, int contractId);
    
    Task<bool> UpdateContractStatusAsync(int contractId, Guid contractorId, string status);

    Task<EmploymentContract?> GetContractByIdAsync(int contractId);
    Task<List<EmploymentContract>> GetUserContractsAsync(Guid userId);



}