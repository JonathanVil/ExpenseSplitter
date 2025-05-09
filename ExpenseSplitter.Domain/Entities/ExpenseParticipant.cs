namespace ExpenseSplitter.Domain.Entities;

public class ExpenseParticipant
{
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    public decimal OwedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public bool IsSettled { get; set; }
    public decimal Balance => PaidAmount - OwedAmount;
    
    // Navigation properties
    public Expense Expense { get; set; } = null!;
    public User User { get; set; } = null!;
}