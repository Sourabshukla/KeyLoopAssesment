
using KeyLoopSalesman.Domain.Entities;

namespace KeyLoopSalesman.Application.Interfaces;

public interface ILeadRepository
{
    Task<IReadOnlyList<Lead>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Lead?> GetByIdAsync(int id,CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeadActivity>> GetActivitiesAsync(int leadId,CancellationToken cancellationToken = default);
    Task<LeadActivity> AddActivityAsync(LeadActivity activity,CancellationToken cancellationToken = default);
}
