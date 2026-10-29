
using KeyLoopSalesman.Application.Dtos;
using KeyLoopSalesman.Application.Interfaces;
using KeyLoopSalesman.Application.Services;
using KeyLoopSalesman.Domain.Entities;
using Xunit;

namespace KeyLoopSalesman.Tests.Services;

public class LeadServiceTests
{
    [Fact]
    public async Task GetAllAsync_WhenLeadsExist_ReturnsLeads()
    {
        // Arrange
        var repository = new FakeLeadRepository();
        var service = new LeadService(repository);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Rahul Sharma", result[0].CustomerName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLeadDoesNotExist_ReturnsNull()
    {
        // Arrange
        var service = new LeadService(new FakeLeadRepository());

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddActivityAsync_WhenTypeIsInvalid_ThrowsException()
    {
        // Arrange
        var service = new LeadService(new FakeLeadRepository());

        var request = new CreateLeadActivityRequest
        {
            ActivityType = "Invalid",
            Notes = "Called customer"
        };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddActivityAsync(1, request));
    }

    [Fact]
    public async Task AddActivityAsync_WithValidInput_ReturnsCreatedActivity()
    {
        // Arrange
        var repository = new FakeLeadRepository();
        var service = new LeadService(repository);

        var request = new CreateLeadActivityRequest
        {
            ActivityType = "Call",
            Notes = "Customer requested a test drive."
        };

        // Act
        var result = await service.AddActivityAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.LeadId);
        Assert.Equal("Call", result.ActivityType);
        Assert.Equal("Customer requested a test drive.", result.Notes);
    }

    [Fact]
    public async Task AddActivityAsync_WithBlankNotes_ThrowsArgumentException()
    {
        // Arrange
        var service = new LeadService(new FakeLeadRepository());

        var request = new CreateLeadActivityRequest
        {
            ActivityType = "Call",
            Notes = "   "
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddActivityAsync(1, request));
    }

    [Fact]
    public async Task AddActivityAsync_WhenLeadDoesNotExist_ReturnsNull()
    {
        // Arrange
        var service = new LeadService(new FakeLeadRepository());

        var request = new CreateLeadActivityRequest
        {
            ActivityType = "Call",
            Notes = "Contacted customer"
        };

        // Act
        var result = await service.AddActivityAsync(999, request);

        // Assert
        Assert.Null(result);
    }

    private sealed class FakeLeadRepository : ILeadRepository
    {
        private readonly Lead _lead = new()
        {
            Id = 1,
            CustomerName = "Rahul Sharma",
            Email = "rahul@example.com",
            VehicleInterest = "Hyundai Creta"
        };

        public Task<IReadOnlyList<Lead>> GetAllAsync(CancellationToken ct = default)=> Task.FromResult<IReadOnlyList<Lead>>(new List<Lead> { _lead });

        public Task<Lead?> GetByIdAsync(int id,CancellationToken ct = default)=> Task.FromResult<Lead?>(id == 1 ? _lead : null);

        public Task<bool> ExistsAsync(int id,CancellationToken ct = default)=> Task.FromResult(id == 1);

        public Task<IReadOnlyList<LeadActivity>> GetActivitiesAsync(int leadId,CancellationToken ct = default)=> Task.FromResult<IReadOnlyList<LeadActivity>>(new List<LeadActivity>());

        public Task<LeadActivity> AddActivityAsync(LeadActivity activity,CancellationToken ct = default)
        {
            activity.Id = 1;
            return Task.FromResult(activity);
        }
    }
}
