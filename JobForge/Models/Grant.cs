namespace JobForge.Models;

public class Grant
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ApplicationStartDate { get; set; }
    public DateTime ApplicationEndDate { get; set; }
    public decimal? Amount { get; set; }
    public Guid AuthorId { get; set; }
}