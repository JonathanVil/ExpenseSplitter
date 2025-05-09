using System.Security.Claims;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Group;

public record AddGroupMemberRequest(Guid GroupId, string UserEmail);

public class AddGroupMemberEndpoint : Endpoint<AddGroupMemberRequest>
{
    private readonly AppDbContext _db;

    public AddGroupMemberEndpoint(AppDbContext db)
    {
        _db = db;
    }
    
    public override void Configure()
    {
        Post("/group/{GroupId}/members");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(AddGroupMemberRequest req, CancellationToken ct)
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
        var newMember = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.UserEmail, ct);
        if (newMember == null)
        {
            ThrowError("User not found", StatusCodes.Status404NotFound);
            return;
        }

        // Check if the user is already a member of the group
        if (group.Memberships.Any(m => m.UserId == newMember.Id))
        {
            ThrowError("User is already a member of that group", StatusCodes.Status409Conflict);
            return;
        }

        // Create membership
        var membership = new GroupMembership
        {
            User = newMember,
            Group = group,
            IsAdmin = false
        };
        _db.GroupMemberships.Add(membership);
        await _db.SaveChangesAsync(ct);
        
        await SendAsync("Member added successfully", StatusCodes.Status200OK, ct);
    }
}