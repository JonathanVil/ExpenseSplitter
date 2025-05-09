using ExpenseSplitter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMembership> GroupMemberships => Set<GroupMembership>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseParticipant> ExpenseParticipants => Set<ExpenseParticipant>();

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
            entity.HasKey(gm => new { gm.UserId, gm.GroupId });

            entity.HasOne(gm => gm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete conflicts

            entity.HasOne(gm => gm.Group)
                .WithMany(g => g.Memberships)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Prevent cascade delete conflicts
        });

        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Amount).HasPrecision(18, 2);

            entity.HasIndex(e => e.CreatedByUserId);
            
            // Configure one-to-many relationship between Group and Expense
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.GroupId);
            
            // Configure one-to-many relationship between User and Expense (creator)
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CreatedByUser)
                .WithMany(u => u.CreatedExpenses)
                .HasForeignKey(e => e.CreatedByUserId);

            // Many-to-many relationship with ExpenseParticipant
            entity.HasMany(e => e.Users)
                .WithMany(u => u.SharedExpenses)
                .UsingEntity<ExpenseParticipant>(
                    j => j.HasOne(ep => ep.User)
                        .WithMany()
                        .HasForeignKey(ep => ep.UserId),
                    j => j.HasOne(ep => ep.Expense)
                        .WithMany()
                        .HasForeignKey(ep => ep.ExpenseId));
        });

        // Configure ExpenseSplit entity
        modelBuilder.Entity<ExpenseParticipant>(entity =>
        {
            entity.Property(e => e.OwedAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);

            // Configure many-to-many relationship for expense participants
            modelBuilder.Entity<ExpenseParticipant>()
                .HasKey(ep => new { ep.ExpenseId, ep.UserId });
                
            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.Expense)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.ExpenseId);
                
            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(ep => ep.UserId);
        });
    }
}