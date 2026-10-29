
using KeyLoopSalesman.Domain.Enums;

namespace KeyLoopSalesman.Domain.Entities;

public class Lead
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string VehicleInterest { get; set; } = string.Empty;

    public LeadStatus Status { get; set; } = LeadStatus.New;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<LeadActivity> Activities { get; set; }
        = new List<LeadActivity>();
}
