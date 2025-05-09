using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Shared.Utils;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record CreateExpenseRequest(Guid GroupId, string Title, decimal Amount, Dictionary<string, decimal> Participants);

public record CreateExpenseRequestWithEqualSplit : CreateExpenseRequest
{
    public CreateExpenseRequestWithEqualSplit(Guid GroupId, string Title, decimal Amount, HashSet<string> participants) : base(GroupId, Title, Amount, participants.ToDictionary(u => u, _ => Amount / participants.Count))
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

        RuleFor(x => x.Participants)
            .NotEmpty()
            .WithMessage("Splits are required");

        RuleForEach(x => x.Participants.Keys)
            .IsValidGuid()
            .WithMessage("All 'Splits' keys must be valid User IDs");
        
        RuleFor(x => x.Participants)
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
        var participants = new List<ExpenseParticipant>();
        foreach (var split in req.Participants)
        {
            var payer = await _db.Users.FindAsync([Guid.Parse(split.Key)], ct);
            if (payer == null)
            {
                ThrowError($"User with ID {split.Key} not found", StatusCodes.Status404NotFound);
                return;
            }
            
            var participant = new ExpenseParticipant
            {
                User = payer,
                UserId = Guid.Parse(split.Key),
                OwedAmount = split.Value
            };
            participants.Add(participant);
        }

        // Create expense
        var expense = new Expense
        {
            Title = req.Title,
            Description = null,
            Amount = req.Amount,
            Date = DateTime.UtcNow,
            CreatedByUser = user,
            Participants = participants,
            Group = group
        };
        
        await _db.Expenses.AddAsync(expense, ct);
        await _db.SaveChangesAsync(ct);

        await SendAsync(new CreateExpenseResponse(expense.Id), cancellation: ct);
    }
}