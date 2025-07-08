namespace JobForge.DbModels;

public class GrantDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ApplicationStartDate { get; set; }
    public DateTime ApplicationEndDate { get; set; }
    public decimal? Amount { get; set; }
}