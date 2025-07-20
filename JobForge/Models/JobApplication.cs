using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using JobForge.DbModels;

namespace JobForge.Models;

public class JobApplication
{
    [Key]
    public int Id { get; set; }

    public int JobOfferId { get; set; }
    public JobOffer JobOffer { get; set; }

    public Guid UserId { get; set; }

    public Guid CvId { get; set; }
    public GeneratedCV CV { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    // Status aplikacji: "Pending", "Rejected", "Accepted", itp.
    [Required]
    public string Status { get; set; } = "Pending";
}
