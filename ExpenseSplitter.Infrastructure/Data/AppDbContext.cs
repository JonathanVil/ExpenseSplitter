using ExpenseSplitter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMembership> GroupMemberships => Set<GroupMembership>();
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
            
            // Many-to-many relationship with Group
            entity.HasMany(u => u.Groups)
                .WithMany(g => g.Members)
                .UsingEntity<GroupMembership>();
        });
        
        // Configure Group entity
        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            entity.Property(g => g.Description).HasMaxLength(500);
        });
        
        // Configure GroupMembership
        modelBuilder.Entity<GroupMembership>(entity =>
        {
            entity.HasKey(gm => new {gm.UserId, gm.GroupId});
            
            entity.HasOne(gm => gm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete conflicts
            
            entity.HasOne(gm => gm.Group)
                .WithMany(g => g.Memberships)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete conflicts
        });
        
        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2);

            entity.HasIndex(e => e.PaidByUserId);
            
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