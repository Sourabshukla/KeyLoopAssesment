using KeyLoopSalesman.Domain.Entities;
using KeyLoopSalesman.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KeyLoopSalesman.Infrastructure.Data;

public class LeadManagementDbContext : DbContext
{
    public LeadManagementDbContext(DbContextOptions<LeadManagementDbContext> options): base(options)
    {

    }

    public DbSet<Lead> Leads => Set<Lead>();

    public DbSet<LeadActivity> LeadActivities=> Set<LeadActivity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("Leads");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomerName).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.VehicleInterest).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasIndex(x => x.CreatedAtUtc);
        });

        modelBuilder.Entity<LeadActivity>(entity =>
        {
            entity.ToTable("LeadActivities");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ActivityType).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(x => x.Notes).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasOne(x => x.Lead).WithMany(x => x.Activities).HasForeignKey(x => x.LeadId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new
            {
                x.LeadId,
                x.CreatedAtUtc,
                x.Id
            });
        });
    }
}
