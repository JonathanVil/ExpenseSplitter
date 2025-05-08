using ExpenseSplitter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            
            // One-to-many relationship with ExpenseSplits
            entity.HasMany(e => e.Splits)
                .WithOne(s => s.Expense)
                .HasForeignKey(s => s.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure ExpenseSplit entity
        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            
            // Many-to-one relationship with User
            entity.HasOne(e => e.User)
                .WithMany(u => u.OwedExpenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete conflicts
        });
    }
}