namespace JobForge.Models;

public class Language
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string LanguageName { get; set; }
    public string ProficiencyLevel { get; set; }

    public int PersonalInformationId { get; set; }
    public PersonalInformation PersonalInformation { get; set; }
}
