using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record ListGroupsResponse(List<GroupDto> Groups);

public record GroupDto(Guid Id, string Name, string? Description, DateTime CreatedAt, bool IsAdmin);

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
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }

        var groupDtos = user.Memberships
            .Select(m => new GroupDto(
                m.Group.Id,
                m.Group.Name,
                m.Group.Description,
                m.Group.CreatedAt,
                m.IsAdmin
            ))
            .ToList();

        await SendAsync(new ListGroupsResponse(groupDtos), cancellation: ct);
    }
}