using JobForge.Models;

namespace JobForge.DbModels;

public class JobApplicationWithCVDto
{
    public int ApplicationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public GeneratedCV CV { get; set; }

}