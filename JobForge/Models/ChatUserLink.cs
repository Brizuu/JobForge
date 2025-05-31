namespace JobForge.Models;

public class ChatUserLink
{
    public int Id { get; set; }
    public Guid FirstUser { get; set; }
    public Guid SecoundUser { get; set; }
}