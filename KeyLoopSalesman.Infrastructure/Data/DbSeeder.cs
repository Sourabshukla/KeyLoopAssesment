
using KeyLoopSalesman.Domain.Entities;
using KeyLoopSalesman.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KeyLoopSalesman.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        LeadManagementDbContext context)
    {
        if (await context.Leads.AnyAsync())
            return;

        var leads = new List<Lead>
        {
            new Lead
            {
                CustomerName = "Rahul Sharma",
                Email = "rahul@example.com",
                Phone = "9876543210",
                VehicleInterest = "Hyundai Creta",
                Status = LeadStatus.New,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new Lead
            {
                CustomerName = "Priya Reddy",
                Email = "priya@example.com",
                Phone = "9876543211",
                VehicleInterest = "Tata Nexon",
                Status = LeadStatus.Contacted,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            },
            new Lead
            {
                CustomerName = "Amit Kumar",
                Email = "amit@example.com",
                Phone = "9876543212",
                VehicleInterest = "Honda City",
                Status = LeadStatus.Qualified,
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        await context.Leads.AddRangeAsync(leads);
        await context.SaveChangesAsync();

        var activities = new List<LeadActivity>
        {
            new LeadActivity
            {
                LeadId = leads[0].Id,
                ActivityType = LeadActivityType.Call,
                Notes = "Initial call attempted. Customer unavailable.",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-6)
            },
            new LeadActivity
            {
                LeadId = leads[0].Id,
                ActivityType = LeadActivityType.Email,
                Notes = "Sent vehicle brochure to customer.",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-4)
            },
            new LeadActivity
            {
                LeadId = leads[1].Id,
                ActivityType = LeadActivityType.Call,
                Notes = "Customer requested a test drive.",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-2)
            }
        };

        await context.LeadActivities.AddRangeAsync(activities);
        await context.SaveChangesAsync();
    }
}
