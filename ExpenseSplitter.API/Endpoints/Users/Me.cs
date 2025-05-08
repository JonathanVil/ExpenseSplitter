using System.Security.Claims;
using FastEndpoints;

namespace ExpenseSplitter.API.Endpoints.Users;


public class MeEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/me");
        Claims(ClaimTypes.Email);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        await SendAsync($"You are {email} and is authorized!", cancellation: ct);
    }
}