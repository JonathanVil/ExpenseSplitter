using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FluentValidation;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record CreateExpenseRequest(string Title, decimal Amount, Dictionary<string, decimal> Splits);

public record CreateExpenseRequestWithEqualSplit : CreateExpenseRequest
{
    public CreateExpenseRequestWithEqualSplit(string Title, decimal Amount, HashSet<string> users) : base(Title, Amount, users.ToDictionary(u => u, _ => Amount / users.Count))
    {
    }
}

public record CreateExpenseResponse(Guid ExpenseId);

public class CreateExpenseRequestValidator : Validator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Amount must be less than or equal to 1,000,000");

        RuleFor(x => x.Splits)
            .NotEmpty()
            .WithMessage("Splits are required");
        
        // splits must sum to Amount
        RuleFor(x => x)
            .Must(x => x.Splits.Values.Sum() == x.Amount)
            .WithMessage("The sum of all splits must equal the total amount");
    }
}

public class CreateExpenseEndpoint : Endpoint<CreateExpenseRequest, CreateExpenseResponse>
{
    private readonly AppDbContext _db;

    public CreateExpenseEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/expenses");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateExpenseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId, ct);
        if (user == null)
        {
            ThrowError("User not found");
            return;
        }

        // Create expense
        var expense = new Expense
        {
            Title = req.Title,
            Description = null,
            Amount = req.Amount,
            Date = DateTime.UtcNow,
            PaidByUser = user,
            Splits = req.Splits.Select(s => new ExpenseSplit
            {
                UserId = Guid.Parse(s.Key),
                Amount = s.Value
            }).ToList()
        };
        
        await _db.Expenses.AddAsync(expense, ct);
        await _db.SaveChangesAsync(ct);

        await SendAsync(new CreateExpenseResponse(expense.Id), cancellation: ct);
    }
}