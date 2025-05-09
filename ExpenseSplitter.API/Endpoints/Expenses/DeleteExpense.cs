using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record DeleteExpenseRequest(Guid GroupId, Guid ExpenseId);

public class DeleteExpenseRequestValidator : Validator<DeleteExpenseRequest>
{
    public DeleteExpenseRequestValidator()
    {
        RuleFor(x => x.ExpenseId)
            .NotEmpty()
            .WithMessage("Expense ID is required");
    }
}

public class DeleteExpenseEndpoint : Endpoint<DeleteExpenseRequest>
{
    private readonly AppDbContext _db;

    public DeleteExpenseEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/group/{GroupId}/expenses/{ExpenseId}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(DeleteExpenseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var group = await _db.Groups
            .Include(g => g.Members)
            .Include(group => group.Expenses)
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

        var expense = group.Expenses.FirstOrDefault(x => x.Id == req.ExpenseId && x.CreatedByUserId == userId);
        if (expense == null)
        {
            ThrowError("Expense not found", StatusCodes.Status404NotFound);
            return;
        }

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync(ct);

        await SendAsync("Expense deleted successfully", StatusCodes.Status200OK, ct);
    }
}