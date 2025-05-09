namespace ExpenseSplitter.Domain.Entities;

public class Group : EntityBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    // Navigation properties
    public List<GroupMembership> Memberships { get; set; } = [];
    public List<User> Members { get; set; } = [];
    public List<Expense> Expenses { get; set; } = [];
}