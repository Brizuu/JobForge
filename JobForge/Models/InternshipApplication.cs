using System;

namespace JobForge.Models
{
    public class InternshipApplication
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime AppliedAt { get; set; }
        public Guid InternshipId { get; set; }  
    }
}