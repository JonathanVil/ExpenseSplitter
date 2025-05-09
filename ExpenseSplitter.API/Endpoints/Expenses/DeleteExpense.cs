using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record DeleteExpenseRequest(Guid ExpenseId);

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
        Delete("/expenses/{ExpenseId}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(DeleteExpenseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync([userId], ct);
        if (user == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }

        var expense = await _db.Expenses
            .FirstOrDefaultAsync(x => x.Id == req.ExpenseId && x.PaidByUserId == userId, ct);
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