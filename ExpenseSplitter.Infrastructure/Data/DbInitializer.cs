using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Infrastructure.Data;

public static class DbInitializer
{
    public static IServiceProvider InitializeDb(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
        
        return serviceProvider;
    }
}