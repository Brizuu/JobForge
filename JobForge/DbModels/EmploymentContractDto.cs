namespace JobForge.DbModels;

public class EmploymentContractDto
{
    public int Id { get; set; }
    public DateTime ContractDate { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyNip { get; set; } = string.Empty;
    public string WorkplaceLocation { get; set; } = string.Empty;
    public string WorkTimeDimension { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string AdditionalEmploymentConditions { get; set; } = string.Empty;
    public DateTime WorkStartDate { get; set; }
    public DateTime? ContractValidFrom { get; set; }
    public DateTime? ContractValidTo { get; set; }
    public bool IsPermanentContract => ContractValidTo == null;
    public string? ContractDocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
