namespace ExpenseSplitter.Domain.Entities;

public class ExpenseSplit : EntityBase
{
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid => PaidAt != null;
    public DateTime? PaidAt { get; set; }
    
    // Navigation properties
    public Expense Expense { get; set; } = null!;
    public User User { get; set; } = null!;
}