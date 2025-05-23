namespace JobForge.DbModels;

public class ArchiveJobOfferDto
{
    public int JobOfferId { get; set; }
    public bool IsArchived { get; set; } = true;
}