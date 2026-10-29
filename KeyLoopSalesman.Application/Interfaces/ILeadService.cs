
using KeyLoopSalesman.Application.Dtos;

namespace KeyLoopSalesman.Application.Interfaces;

public interface ILeadService
{
    Task<IReadOnlyList<LeadDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeadDetailsDto?> GetByIdAsync(int id,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeadActivityDto>?> GetActivitiesAsync(int leadId,CancellationToken cancellationToken = default);
    Task<LeadActivityDto?> AddActivityAsync(int leadId,CreateLeadActivityRequest request,CancellationToken cancellationToken = default);
}
