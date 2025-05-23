namespace JobForge.DbModels;

public class EducationDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string SchoolName { get; set; }
    public string Major { get; set; }
    public DateTime EducationDateStart { get; set; }
    public DateTime EducationDateEnd { get; set; }
}
