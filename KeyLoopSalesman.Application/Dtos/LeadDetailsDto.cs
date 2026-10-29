
namespace KeyLoopSalesman.Application.Dtos;

public class LeadDetailsDto : LeadDto
{
    public List<LeadActivityDto> Activities { get; set; } = new();
}
