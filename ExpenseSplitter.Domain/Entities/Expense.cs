namespace ExpenseSplitter.Domain.Entities;

public class Expense : EntityBase
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid PaidByUserId { get; set; }
    
    // Navigation properties
    public List<ExpenseSplit> Splits { get; set; } = [];
    public User PaidByUser { get; set; } = null!;
}