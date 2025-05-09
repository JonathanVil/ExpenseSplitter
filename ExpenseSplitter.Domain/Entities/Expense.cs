namespace ExpenseSplitter.Domain.Entities;

public class Expense : EntityBase
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid GroupId { get; set; }
    public Guid CreatedByUserId { get; set; }
    
    // Navigation properties
    public Group Group { get; set; } = null!;
    public List<ExpenseParticipant> Participants { get; set; } = [];
    public User CreatedByUser { get; set; } = null!;
    public List<User> Users { get; set; } = [];
}