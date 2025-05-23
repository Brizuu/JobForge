namespace JobForge.Models;

public class JobOfferTechnology
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int JobOfferId { get; set; }
    // public JobOffer JobOffer { get; set; }
}