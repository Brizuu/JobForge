namespace JobForge.DbModels;

public class LanguageDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string LanguageName { get; set; }
    public string ProficiencyLevel { get; set; }
}
