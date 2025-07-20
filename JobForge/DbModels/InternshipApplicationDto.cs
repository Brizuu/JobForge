namespace JobForge.DbModels;

public class InternshipApplicationDto
{
    public Guid InternshipId { get; set; }      
    
    public string ApplicantName { get; set; }   
    public string ContactEmail { get; set; }    
    public string ContactPhone { get; set; }    

    public string CoverLetter { get; set; }     
    
}