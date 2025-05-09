using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record RemoveGroupMemberRequest(Guid GroupId, string UserEmail);

public class RemoveGroupMemberEndpoint : Endpoint<RemoveGroupMemberRequest>
{
    private readonly AppDbContext _db;

    public RemoveGroupMemberEndpoint(AppDbContext db)
    {
        _db = db;
    }
    
    public override void Configure()
    {
        Delete("/group/{GroupId}/members");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(RemoveGroupMemberRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var group = await _db.Groups
            .Include(g => g.Memberships)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == req.GroupId, ct);

        // Check if the group exists
        if (group == null)
        {
            ThrowError("Group not found", StatusCodes.Status404NotFound);
            return;
        }

        // Check if the user is an admin of the group
        if (group.Memberships.All(m => m.UserId != userId))
        {
            ThrowError("You are not an admin of that group", StatusCodes.Status403Forbidden);
            return;
        }
        
        // Check if the user exists
        var member = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.UserEmail, ct);
        if (member == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }

        // Check if the user is already a member of the group
        var membership = group.Memberships.FirstOrDefault(m => m.UserId == member.Id);
        if (membership == null)
        {
            ThrowError("User is not a member of that group", StatusCodes.Status409Conflict);
            return;
        }
        
        // Check if the user is the last admin of the group
        if (membership.IsAdmin && group.Memberships.Count(m => m.IsAdmin) == 1)
        {
            ThrowError("You cannot remove the last admin of the group", StatusCodes.Status403Forbidden);
            return;
        }
        
        _db.GroupMemberships.Remove(membership);
        await _db.SaveChangesAsync(ct);
        
        await SendAsync("Member removed successfully", StatusCodes.Status200OK, ct);
    }
}