using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using JobForge.DbModels;

namespace JobForge.Models;

public class JobApplication
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public int CvId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AppliedAt { get; set; }
    public string Status { get; set; }

    public JobOffer JobOffer { get; set; }

    [NotMapped]
    public object? DeserializedCv { get; set; }
}
