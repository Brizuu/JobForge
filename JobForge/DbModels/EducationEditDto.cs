namespace JobForge.DbModels;

public class EducationEditDto
{
    public Guid Id { get; set; }
    public string? SchoolName { get; set; }
    public string? Specialization { get; set; }
    public DateTime? EducationDateStart { get; set; }
    public DateTime? EducationDateEnd { get; set; }
}
