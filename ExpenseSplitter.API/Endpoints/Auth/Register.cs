using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Shared.Utils;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Auth;

public record RegisterRequest(string Email, string Password);

public sealed class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");
    }
}

public sealed class RegisterEndpoint : Endpoint<RegisterRequest>
{
    private readonly AppDbContext _db;

    public RegisterEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
        {
            AddError(r => r.Email, "Email already registered");
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        var user = new User
        {
            Email = req.Email,
            PasswordHash = AuthUtils.HashPassword(req.Password)
        };

        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}