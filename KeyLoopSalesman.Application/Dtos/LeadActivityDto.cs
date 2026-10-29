
namespace KeyLoopSalesman.Application.Dtos;

public class LeadActivityDto
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
