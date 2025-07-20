namespace JobForge.DbModels;

public class JobOfferTechnologyDto
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ExperienceLevel { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}