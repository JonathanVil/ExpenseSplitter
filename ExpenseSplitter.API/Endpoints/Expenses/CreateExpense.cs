using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Shared.Utils;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record CreateExpenseRequest(Guid GroupId, string Title, decimal Amount)
{
    public List<Guid>? Participants { get; init; }
    public Dictionary<Guid, decimal>? Shares { get; init; }
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

        RuleForEach(x => x.Participants)
            .NotEmpty()
            .WithMessage("All Participants must be valid User IDs");

        RuleFor(x => x.Shares)
            .Must((request, shares) => shares!.Values.Sum() <= request.Amount)
            .When(x => x.Shares != null)
            .WithMessage("The sum of all shares cannot exceed the total amount");
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

        // Get users that participate in the expense
        var users = new List<User>();
        if (req.Participants == null)
        {
            users = group.Members.ToList();
        }
        else
        {
            users = group.Members.Where(u => req.Participants.Contains(u.Id)).ToList();

            if (req.Participants.Count > users.Count)
            {
                ThrowError("One or more participants are not members of that group", StatusCodes.Status400BadRequest);
                return;
            }
        }

        // Check if any of the shares are not members of the group
        if (req.Shares != null && req.Shares.Keys.Any(u => users.All(x => x.Id != u)))
        {
            ThrowError("One or more participants are not members of that group", StatusCodes.Status400BadRequest);
            return;
        }

        // Create the parts of the expense
        var participants = new List<ExpenseParticipant>();
        var standardShare = req.Amount / users.Count - (req.Shares?.Values.Sum() ?? 0);
        foreach (var participant in users)
        {
            var shareAmount = req.Shares?.TryGetValue(participant.Id, out var reqShare) is true
                ? reqShare
                : standardShare;
            if (shareAmount <= 0)
            {
                ThrowError("Share amount must be greater than 0", StatusCodes.Status400BadRequest);
                return;
            }

            var part = new ExpenseParticipant
            {
                OwedAmount = shareAmount,
                PaidAmount = 0,
                IsSettled = false,
                User = participant
            };
            participants.Add(part);
        }
        
        // Check if the total amount matches the parts of the expense
        var totalAmount = participants.Sum(p => p.OwedAmount);
        if (totalAmount != req.Amount)
        {
            ThrowError("Total amount does not match the parts of the expense", StatusCodes.Status400BadRequest);
            return;
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