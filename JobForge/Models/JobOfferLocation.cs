namespace JobForge.Models;

public class JobOfferLocation
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    
    
    public string LocationType { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}