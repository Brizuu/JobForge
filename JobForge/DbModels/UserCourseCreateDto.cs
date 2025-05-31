namespace JobForge.DbModels;

public class UserCourseCreateDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; }
    public string CourseDescription { get; set; }
    public double CompletionPercentage { get; set; }
}