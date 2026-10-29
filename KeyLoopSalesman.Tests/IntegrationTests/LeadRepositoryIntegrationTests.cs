
using KeyLoopSalesman.Domain.Entities;
using KeyLoopSalesman.Domain.Enums;
using KeyLoopSalesman.Infrastructure.Data;
using KeyLoopSalesman.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyLoopSalesman.Tests.IntegrationTests;

public class LeadRepositoryIntegrationTests
{
    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;
          Database=KeyLoopSalesman_IntegrationTests;
          Trusted_Connection=True;
          TrustServerCertificate=True";

    private static DbContextOptions<LeadManagementDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<LeadManagementDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
    }

    [Fact]
    public async Task AddActivityAsync_PersistsActivityAcrossDbContexts()
    {
        // Arrange: create and migrate the separate test database
        var options = CreateOptions();
        int leadId = 0;

        await using (var setupContext =
            new LeadManagementDbContext(options))
        {
            await setupContext.Database.MigrateAsync();

            var lead = new Lead
            {
                CustomerName = "Integration Test Customer",
                Email = $"test-{Guid.NewGuid():N}@example.com",
                Phone = "9000000000",
                VehicleInterest = "Hyundai Creta",
                Status = LeadStatus.New,
                CreatedAtUtc = DateTime.UtcNow
            };

            setupContext.Leads.Add(lead);
            await setupContext.SaveChangesAsync();

            leadId = lead.Id;
        }

        try
        {
            // Act: save follow-up using the real repository
            await using (var writeContext =
                new LeadManagementDbContext(options))
            {
                var repository = new LeadRepository(writeContext);

                await repository.AddActivityAsync(
                    new LeadActivity
                    {
                        LeadId = leadId,
                        ActivityType = LeadActivityType.Call,
                        Notes = "Customer requested a test drive.",
                        CreatedAtUtc = DateTime.UtcNow
                    });
            }

            // Assert: open a fresh DbContext and retrieve the data
            await using (var readContext =
                new LeadManagementDbContext(options))
            {
                var repository = new LeadRepository(readContext);

                var activities =
                    await repository.GetActivitiesAsync(leadId);

                Assert.Single(activities);
                Assert.Equal(leadId, activities[0].LeadId);
                Assert.Equal(LeadActivityType.Call,
                    activities[0].ActivityType);
                Assert.Equal(
                    "Customer requested a test drive.",
                    activities[0].Notes);
            }
        }
        finally
        {
            // Clean up only records belonging to this test
            await using var cleanupContext =
                new LeadManagementDbContext(options);

            var activities = await cleanupContext.LeadActivities
                .Where(x => x.LeadId == leadId)
                .ToListAsync();

            cleanupContext.LeadActivities.RemoveRange(activities);

            var lead = await cleanupContext.Leads
                .FindAsync(leadId);

            if (lead is not null)
                cleanupContext.Leads.Remove(lead);

            await cleanupContext.SaveChangesAsync();
        }
    }
}
