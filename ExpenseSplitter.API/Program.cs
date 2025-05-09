using ExpenseSplitter.Infrastructure.Data;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();

// Add Aspire
bld.AddServiceDefaults();

bld.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");

bld.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = bld.Configuration["Auth:JwtSecret"])
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

var app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Services.InitializeDb();

app.Run();