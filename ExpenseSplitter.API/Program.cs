using ExpenseSplitter.Infrastructure;
using FastEndpoints;
using FastEndpoints.Security;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = "i'm a secret dont look at me")
    .AddAuthorization()
    .AddFastEndpoints();

bld.Services.AddInfrastructureServices();

var app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints();
app.Run();