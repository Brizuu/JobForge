using System;

namespace JobForge.Models
{
    public class Internship
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid AuthorId { get; set; }
        public decimal? Salary { get; set; }
    }
}