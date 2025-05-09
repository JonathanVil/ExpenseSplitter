using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Shared.Utils;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record CreateExpenseRequest(Guid GroupId, string Title, decimal Amount, Dictionary<string, decimal> Splits);

public record CreateExpenseRequestWithEqualSplit : CreateExpenseRequest
{
    public CreateExpenseRequestWithEqualSplit(Guid GroupId, string Title, decimal Amount, HashSet<string> users) : base(GroupId, Title, Amount, users.ToDictionary(u => u, _ => Amount / users.Count))
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

        RuleForEach(x => x.Splits.Keys)
            .IsValidGuid()
            .WithMessage("All 'Splits' keys must be valid User IDs");
        
        RuleFor(x => x.Splits)
            .Must((request, splits) => splits.Values.Sum() == request.Amount)
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
        Post("/group/{GroupId}/expenses");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateExpenseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var group = await _db.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == req.GroupId, ct);
        if (group == null)
        {
            ThrowError("Group not found", StatusCodes.Status404NotFound);
            return;
        }
        
        var user = group.Members.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            ThrowError("You are not a member of that group", StatusCodes.Status403Forbidden);
            return;
        }
        
        // Create splits
        var splits = new List<ExpenseSplit>();
        foreach (var split in req.Splits)
        {
            var payer = await _db.Users.FindAsync([Guid.Parse(split.Key)], ct);
            if (payer == null)
            {
                ThrowError($"User with ID {split.Key} not found", StatusCodes.Status404NotFound);
                return;
            }
            
            var expenseSplit = new ExpenseSplit
            {
                User = payer,
                UserId = Guid.Parse(split.Key),
                Amount = split.Value
            };
            splits.Add(expenseSplit);
        }

        // Create expense
        var expense = new Expense
        {
            Title = req.Title,
            Description = null,
            Amount = req.Amount,
            Date = DateTime.UtcNow,
            PaidByUser = user,
            Splits = splits,
            Group = group
        };
        
        await _db.Expenses.AddAsync(expense, ct);
        await _db.SaveChangesAsync(ct);

        await SendAsync(new CreateExpenseResponse(expense.Id), cancellation: ct);
    }
}