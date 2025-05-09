using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Expenses;

public record ExpenseDto(Guid Id, string Title, decimal Amount, DateTime Date);
public record ListExpensesResponse(List<ExpenseDto> Expenses);

public class ListExpensesEndpoint : EndpointWithoutRequest<ListExpensesResponse>
{
    private readonly AppDbContext _db;

    public ListExpensesEndpoint(AppDbContext db)
    {
        _db = db;
    }
    
    public override void Configure()
    {
        Get("/expenses");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var expenses = await _db.Expenses
            .Where(e => e.PaidByUser.Id == userId)
            .Select(e => new ExpenseDto(
                e.Id,
                e.Title,
                e.Amount,
                e.Date
            ))
            .ToListAsync(ct);
        
        await SendAsync(new ListExpensesResponse(expenses), cancellation: ct);
    }
}