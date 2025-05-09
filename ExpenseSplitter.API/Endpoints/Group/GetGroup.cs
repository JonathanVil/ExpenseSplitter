using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record GetGroupRequest(Guid Id);

public record GetGroupResponse(GroupDto Group);

public record GroupDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsAdmin { get; init; }
    public required Dictionary<string, string> Members { get; init; }
}

public class GetGroupEndpoint : Endpoint<GetGroupRequest, GetGroupResponse>
{
    private readonly AppDbContext _db;
    
    public GetGroupEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/group/{Id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(GetGroupRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var group = await _db.Groups
            .Include(g => g.Memberships)
            .ThenInclude(m => m.User)
            .Include(g => g.Expenses)
            .FirstOrDefaultAsync(g => g.Id == req.Id, ct);

        if (group == null)
        {
            ThrowError("Group not found", StatusCodes.Status404NotFound);
            return;
        }

        var membership = group.Memberships.FirstOrDefault(m => m.UserId == userId);
        if (membership == null)
        {
            ThrowError("You are not a member of that group", StatusCodes.Status403Forbidden);
            return;
        }

        var dto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedAt = group.CreatedAt,
            IsAdmin = membership.IsAdmin,
            Members = group.Memberships.ToDictionary(m => m.User.Id.ToString(), m => m.User.Email)
        };
        
        await SendAsync(new GetGroupResponse(dto), cancellation: ct);
    }
}