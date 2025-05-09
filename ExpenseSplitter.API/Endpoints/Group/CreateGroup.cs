using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FluentValidation;

namespace ExpenseSplitter.API.Endpoints.Group;

public record CreateGroupRequest(string Name);

public record CreateGroupResponse(Guid Id);

public sealed class CreateGroupRequestValidator : Validator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}

public class CreateGroupEndpoint : Endpoint<CreateGroupRequest, CreateGroupResponse>
{
    private readonly AppDbContext _db;

    public CreateGroupEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/group");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateGroupRequest req, CancellationToken ct)
    {
        // Get the current user ID from claims
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        // Create a new Group entity
        var group = new Domain.Entities.Group
        {
            Name = req.Name
        };

        // Create a membership record for the creator
        var membership = new Domain.Entities.GroupMembership
        {
            UserId = Guid.Parse(userId),
            GroupId = group.Id,
            IsAdmin = true // Make the creator an admin
        };

        // Add both to the database
        _db.Add(group);
        _db.Add(membership);

        await _db.SaveChangesAsync(ct);

        // Return the ID of the newly created group
        await SendAsync(new CreateGroupResponse(group.Id), cancellation: ct);
    }
}