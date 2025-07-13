namespace JobForge.DbModels;

public class LanguageEditDto
{
    public Guid Id { get; set; }
    public string? LanguageName { get; set; }
    public int? ProficiencyLevel { get; set; }
    public string? AdditionalDescription { get; set; }
}
