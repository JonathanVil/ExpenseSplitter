namespace ExpenseSplitter.Domain.Entities;

public class User : EntityBase
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    // Navigation properties
    public List<GroupMembership> Memberships { get; set; } = [];
    public List<Group> Groups { get; set; } = [];
    public List<Expense> CreatedExpenses { get; set; } = [];
    public List<Expense> SharedExpenses { get; set; } = [];
    public List<ExpenseParticipant> Participations { get; set; } = [];
}