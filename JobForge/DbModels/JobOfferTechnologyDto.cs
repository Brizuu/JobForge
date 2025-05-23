namespace JobForge.DbModels;

public class JobOfferTechnologyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int JobOfferId { get; set; }
    // public JobOfferDto JobOffer { get; set; }
}