using JobForge.DbModels;

namespace JobForge.Models;

public class JobApplication
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public Guid UserId { get; set; }

    public PersonalInformation PersonalInformation { get; set; }

    public string Status { get; set; }
    public DateTime ApplicationDate { get; set; }
}