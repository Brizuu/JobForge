namespace JobForge.Models;

public class PersonalData
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public string LinkedinUrl { get; set; }
    public string Summary { get; set; }
    public List<string> TechnicalSkills { get; set; } = new();
    public List<string> SoftSkills { get; set; } = new();
    public List<string> Interests { get; set; } = new();
    public List<string> Certificates { get; set; } = new();
    public List<string> Courses { get; set; } = new();
}
