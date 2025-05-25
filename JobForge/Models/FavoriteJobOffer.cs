namespace JobForge.Models;

public class FavoriteJobOffer
{
    public Guid UserId { get; set; }
    public int JobOfferId { get; set; }
    public DateTime AddedAt { get; set; }
}