
using KeyLoopSalesman.Domain.Enums;

namespace KeyLoopSalesman.Domain.Entities;

public class LeadActivity
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public LeadActivityType ActivityType { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Lead Lead { get; set; } = null!;
}
