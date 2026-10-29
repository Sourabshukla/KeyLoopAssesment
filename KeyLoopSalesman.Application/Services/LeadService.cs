
using KeyLoopSalesman.Application.Dtos;
using KeyLoopSalesman.Application.Interfaces;
using KeyLoopSalesman.Domain.Entities;
using KeyLoopSalesman.Domain.Enums;

namespace KeyLoopSalesman.Application.Services;

public class LeadService : ILeadService
{
    private readonly ILeadRepository _repository;

    public LeadService(ILeadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LeadDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var leads = await _repository.GetAllAsync(cancellationToken);
        return leads.Select(MapLead).ToList();
    }
    public async Task<LeadDetailsDto?> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return null;
        var lead = await _repository.GetByIdAsync(id, cancellationToken);
        if (lead is null)
            return null;
        var activities = await _repository.GetActivitiesAsync(id, cancellationToken);

        return new LeadDetailsDto
        {
            Id = lead.Id,
            CustomerName = lead.CustomerName,
            Email = lead.Email,
            Phone = lead.Phone,
            VehicleInterest = lead.VehicleInterest,
            Status = lead.Status.ToString(),
            CreatedAtUtc = lead.CreatedAtUtc,
            Activities = activities.Select(MapActivity).ToList()
        };
    }
    public async Task<IReadOnlyList<LeadActivityDto>?> GetActivitiesAsync(int leadId,CancellationToken cancellationToken = default)
    {
        if (leadId <= 0 ||!await _repository.ExistsAsync(leadId, cancellationToken))
            return null;
        var activities = await _repository.GetActivitiesAsync(leadId, cancellationToken);
        return activities.Select(MapActivity).ToList();
    }


    public async Task<LeadActivityDto?> AddActivityAsync(int leadId,CreateLeadActivityRequest request,CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentException("Activity details are required.");
        // Validate lead ID
        if (leadId <= 0)
            return null;
        // Validate notes
        var notes = request.Notes?.Trim();
        if (string.IsNullOrWhiteSpace(notes))
            throw new ArgumentException("Follow-up notes cannot be empty.");

        if (notes.Length > 1000)
            throw new ArgumentException("Follow-up notes cannot exceed 1000 characters.");

        // Validate supported activity types
        if (!Enum.TryParse<LeadActivityType>(request.ActivityType, true, out var activityType) ||
            !Enum.IsDefined(activityType) || !string.Equals(activityType.ToString(),request.ActivityType,StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Activity type must be Call, Email, Meeting or Other.");
        }

        // Verify that the lead exists
        var leadExists = await _repository.ExistsAsync(leadId, cancellationToken);
        if (!leadExists)
            return null;
        // Create follow-up activity
        var activity = new LeadActivity
        {
            LeadId = leadId,
            ActivityType = activityType,
            Notes = notes,
            CreatedAtUtc = DateTime.UtcNow
        };
        // Persist through repository
        var savedActivity = await _repository.AddActivityAsync(activity, cancellationToken);

        return MapActivity(savedActivity);
    }
    private static LeadDto MapLead(Lead lead) => new()
    {
        Id = lead.Id,
        CustomerName = lead.CustomerName,
        Email = lead.Email,
        Phone = lead.Phone,
        VehicleInterest = lead.VehicleInterest,
        Status = lead.Status.ToString(),
        CreatedAtUtc = lead.CreatedAtUtc
    };
    private static LeadActivityDto MapActivity(LeadActivity activity) => new()
    {
        Id = activity.Id,
        LeadId = activity.LeadId,
        ActivityType = activity.ActivityType.ToString(),
        Notes = activity.Notes,
        CreatedAtUtc = activity.CreatedAtUtc
    };
}
