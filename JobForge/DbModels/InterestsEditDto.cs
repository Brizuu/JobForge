namespace JobForge.DbModels;

public class InterestsEditDto
{
    public Guid Id { get; set; }
    public string? InterestName { get; set; }
    public int? ProficiencyLevel { get; set; }
    public string? AdditionalDescription { get; set; }
}