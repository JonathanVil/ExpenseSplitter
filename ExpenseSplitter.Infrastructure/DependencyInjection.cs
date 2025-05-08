using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // add AppDbContext postgres
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=expensesplitter;Username=testuser;Password=testpassword");
        });
        
        return services;
    }
}