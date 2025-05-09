using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record DeleteGroupRequest(Guid Id);

public class DeleteGroupEndpoint : Endpoint<DeleteGroupRequest>
{
    private readonly AppDbContext _db;

    public DeleteGroupEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/group/{Id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(DeleteGroupRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users
            .Include(user => user.Memberships)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }
        
        var group = await _db.Groups.FindAsync([req.Id], cancellationToken: ct);
        if (group == null)
        {
            ThrowError("No group with that ID was found", StatusCodes.Status404NotFound);
            return;
        }
        
        // Check if the user is an admin of the group
        if (!user.Memberships.Any(m => m.GroupId == req.Id && m.IsAdmin))
        {
            ThrowError("You are not an admin of that group", StatusCodes.Status403Forbidden);
            return;
        }
        
        // Delete the group
        _db.Groups.Remove(group);
        await _db.SaveChangesAsync(ct);
        
        await SendAsync("Group deleted successfully", StatusCodes.Status200OK, ct);
    }
}