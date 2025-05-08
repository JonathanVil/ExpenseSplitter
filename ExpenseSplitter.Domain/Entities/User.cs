namespace ExpenseSplitter.Domain.Entities;

public class User : EntityBase
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    // Navigation properties
    public List<ExpenseSplit> OwedExpenses { get; set; } = [];
    public List<ExpenseSplit> PaidExpenses { get; set; } = [];
}