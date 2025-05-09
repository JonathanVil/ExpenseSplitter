namespace ExpenseSplitter.Domain.Entities;

public class GroupMembership
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}