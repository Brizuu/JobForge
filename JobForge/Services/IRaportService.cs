namespace JobForge.Services;

public interface IRaportService
{
    Task<int> CountUsersInCompanyAsync(Guid companyId);
}