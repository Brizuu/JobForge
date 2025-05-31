namespace JobForge.Models;

public class UserCourse
{
    public Guid UserId { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; }
    public string CourseDescription { get; set; }
    public double CompletionPercentage { get; set; }
}