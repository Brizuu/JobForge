namespace JobForge.DbModels;

public class FavoriteJobOfferDetailDto
{
    public int JobOfferId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public DateTime AddedAt { get; set; }
}