using ExpenseSplitter.Infrastructure;
using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = bld.Configuration["Auth:JwtSecret"])
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

bld.Services.AddInfrastructureServices();

var app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Services.InitializeDb();

app.Run();