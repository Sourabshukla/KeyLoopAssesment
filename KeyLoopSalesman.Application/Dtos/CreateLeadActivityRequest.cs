
using System.ComponentModel.DataAnnotations;

namespace KeyLoopSalesman.Application.Dtos;

public class CreateLeadActivityRequest
{
    [Required(ErrorMessage = "Activity type is required.")]
    [MaxLength(20)]
    public string ActivityType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Notes are required.")]
    [StringLength(1000, MinimumLength = 1)]
    public string Notes { get; set; } = string.Empty;
}
