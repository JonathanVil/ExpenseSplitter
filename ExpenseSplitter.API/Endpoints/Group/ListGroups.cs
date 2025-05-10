using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record ListGroupsResponse(List<GroupListItem> Groups);

public record GroupListItem(Guid Id, string Name, string? Description, DateTime CreatedAt, bool IsAdmin, List<string> Members);

public class ListGroupsEndpoint : EndpointWithoutRequest<ListGroupsResponse>
{
    private readonly AppDbContext _db;

    public ListGroupsEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/group");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _db.Users
            .Include(u => u.Memberships)
            .ThenInclude(m => m.Group)
            .ThenInclude(g => g.Members)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }

        var groupDtos = user.Memberships
            .Select(m => new GroupListItem(
                m.Group.Id,
                m.Group.Name,
                m.Group.Description,
                m.Group.CreatedAt,
                m.IsAdmin,
                m.Group.Members.Select(u => u.Email).ToList()
            ))
            .ToList();

        await SendAsync(new ListGroupsResponse(groupDtos), cancellation: ct);
    }
}