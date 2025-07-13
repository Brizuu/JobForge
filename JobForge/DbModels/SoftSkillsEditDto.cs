namespace JobForge.DbModels;

public class SoftSkillsEditDto
{
    public Guid Id { get; set; }
    public string? SkillName { get; set; }
    public int? ProficiencyLevel { get; set; }
    public string? AdditionalDescription { get; set; }
}