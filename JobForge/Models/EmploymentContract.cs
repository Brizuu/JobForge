namespace JobForge.Models;

public class EmploymentContract
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid?  Contractor { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime ContractDate { get; set; }
    public string EmploymentType { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyNip { get; set; }
    public string WorkplaceLocation { get; set; }
    public string WorkTimeDimension { get; set; }
    public decimal Salary { get; set; }
    public string AdditionalEmploymentConditions { get; set; }
    public DateTime WorkStartDate { get; set; }
    public DateTime? ContractValidFrom { get; set; }
    public DateTime? ContractValidTo { get; set; }
    public string? ContractDocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
