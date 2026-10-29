
using KeyLoopSalesman.Application.Dtos;
using KeyLoopSalesman.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KeyLoopSalesman.Api.Controllers;

[ApiController]
[Route("api/leads")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(ILeadService leadService,ILogger<LeadsController> logger)
    {
        _leadService = leadService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LeadDto>>> GetAll(CancellationToken cancellationToken)
    {
        var leads = await _leadService.GetAllAsync(cancellationToken);
        return Ok(leads);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeadDetailsDto>> GetById(int id,CancellationToken cancellationToken)
    {
        var lead = await _leadService.GetByIdAsync(id, cancellationToken);
        if (lead is null) return NotFound(new ProblemDetails
            {
                Title = "Lead not found",
                Detail = $"No lead exists with ID {id}.",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(lead);
    }

    [HttpGet("{id:int}/activities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<LeadActivityDto>>> GetActivities(int id,CancellationToken cancellationToken)
    {
        var activities = await _leadService.GetActivitiesAsync(id, cancellationToken);

        if (activities is null)
            return NotFound(new ProblemDetails
            {
                Title = "Lead not found",
                Detail = $"No lead exists with ID {id}.",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(activities);
    }


    [HttpPost("{id:int}/activities")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeadActivityDto>> AddActivity(int id,[FromBody] CreateLeadActivityRequest request,CancellationToken cancellationToken)
    {
        var activity = await _leadService.AddActivityAsync(id, request, cancellationToken);
        if (activity is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Lead not found",
                Detail = $"No lead exists with ID {id}.",
                Status = StatusCodes.Status404NotFound
            });
        }
        _logger.LogInformation("Follow-up activity {ActivityId} added to lead {LeadId}",activity.Id,id);

        return CreatedAtAction(nameof(GetActivities),new { id },activity);
    }

}
