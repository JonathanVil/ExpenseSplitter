using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record ListExpensesRequest(Guid GroupId);

public record ExpenseDto(Guid Id, string Title, decimal Amount, DateTime Date);

public record ListExpensesResponse(List<ExpenseDto> Expenses);

public class ListExpensesEndpoint : Endpoint<ListExpensesRequest, ListExpensesResponse>
{
    private readonly AppDbContext _db;

    public ListExpensesEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/group/{GroupId}/expenses");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(ListExpensesRequest req, CancellationToken ct)
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

        var dtos = group.Expenses
            .Where(e => e.PaidByUserId == userId)
            .Select(e => new ExpenseDto(
                e.Id,
                e.Title,
                e.Amount,
                e.Date
            ))
            .ToList();

        await SendAsync(new ListExpensesResponse(dtos), cancellation: ct);
    }
}