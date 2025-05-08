using System.Security.Claims;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Shared.Utils;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.API.Endpoints.Users;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, Guid UserId);

sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly AppDbContext _db;

    public LoginEndpoint(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user == null || !AuthUtils.VerifyPassword(req.Password, user.PasswordHash))
        {
            ThrowError("Invalid credentials");
            return;
        }
        
        var jwtSecret = Config["Auth:JwtSecret"] ?? throw new Exception("JWT secret not found");

        // Create JWT token with claims
        var jwtToken = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSecret;
            options.ExpireAt = DateTime.UtcNow.AddDays(30);
            options.User[ClaimTypes.NameIdentifier] = user.Id.ToString();
            options.User[ClaimTypes.Email] = user.Email;
        });

        await SendAsync(new LoginResponse(jwtToken, user.Id), cancellation: ct);
    }
}