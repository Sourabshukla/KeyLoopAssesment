
using KeyLoopSalesman.Application.Interfaces;
using KeyLoopSalesman.Domain.Entities;
using KeyLoopSalesman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyLoopSalesman.Infrastructure.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly LeadManagementDbContext _context;

    public LeadRepository(LeadManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Leads.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ThenByDescending(x => x.Id).ToListAsync(cancellationToken);
    }
    public async Task<Lead?> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        return await _context.Leads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<bool> ExistsAsync(int id,CancellationToken cancellationToken = default)
    {
        return await _context.Leads.AnyAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<LeadActivity>> GetActivitiesAsync(int leadId,CancellationToken cancellationToken = default)
    {
        return await _context.LeadActivities
            .AsNoTracking()
            .Where(x => x.LeadId == leadId)
            .OrderBy(x => x.CreatedAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
    public async Task<LeadActivity> AddActivityAsync(LeadActivity activity,CancellationToken cancellationToken = default)
    {
        await _context.LeadActivities.AddAsync(activity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return activity;
    }
}
