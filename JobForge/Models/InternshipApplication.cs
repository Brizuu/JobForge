using System;

namespace JobForge.Models
{
    public class InternshipApplication
    {
        public Guid Id { get; set; }
        public Guid InternshipId { get; set; }      
        public Guid UserId { get; set; }             
    
        public string ApplicantName { get; set; }   
        public string ContactEmail { get; set; }    
        public string ContactPhone { get; set; }    

        public string CoverLetter { get; set; }     
        
        public string Status { get; set; } = "Pending";

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow; 
    }
}