using System;

namespace JobForge.Models
{
    public class Internship
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; }       
        public string Description { get; set; }  
        public string CompanyName { get; set; }   

        public List<InternshipLocation> Locations { get; set; } = new();
        
        public bool IsArchived { get; set; } = true;

        public DateTime StartDate { get; set; }   
        public DateTime EndDate { get; set; }     
        public string Duration { get; set; }      
        public string EmploymentType { get; set; } 
        public decimal? Stipend { get; set; }     
        public string Requirements { get; set; }  
        public string Benefits { get; set; }      
        public string ContactEmail { get; set; }  
    }
}