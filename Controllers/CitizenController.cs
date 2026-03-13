using CitizensApi.Models;
using CitizensApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitizensApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private readonly CitizenFileService _citizenFileService;
    private readonly ExternalObjectService _externalObjectService;
    private readonly ILogger<CitizenController> _logger;

    public CitizenController(
        CitizenFileService citizenFileService,
        ExternalObjectService externalObjectService,
        ILogger<CitizenController> logger)
    {
        _citizenFileService = citizenFileService;
        _externalObjectService = externalObjectService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<List<Citizen>> GetAll()
    {
        var citizens = _citizenFileService.GetAll();
        return Ok(citizens);
    }

    [HttpGet("{ci}")]
    public ActionResult<Citizen> GetByCI(string ci)
    {
        var citizens = _citizenFileService.GetAll();
        var citizen = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizen == null)
        {
            return NotFound("Citizen not found");
        }

        return Ok(citizen);
    }

    [HttpPost]
    public async Task<ActionResult<Citizen>> Create(CreateCitizenRequest request)
    {
        var citizens = _citizenFileService.GetAll();

        if (citizens.Any(c => c.CI == request.CI))
        {
            return BadRequest("Citizen with this CI already exists");
        }

        var bloodGroups = new[] { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
        var random = new Random();

        var citizen = new Citizen
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            CI = request.CI,
            BloodGroup = bloodGroups[random.Next(bloodGroups.Length)],
            PersonalAsset = await _externalObjectService.GetRandomObjectNameAsync()
        };

        citizens.Add(citizen);
        _citizenFileService.SaveAll(citizens);

        _logger.LogInformation("Citizen created");

        return Ok(citizen);
    }

    [HttpPut("{ci}")]
    public ActionResult<Citizen> Update(string ci, UpdateCitizenRequest request)
    {
        var citizens = _citizenFileService.GetAll();
        var citizen = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizen == null)
        {
            return NotFound("Citizen not found");
        }

        citizen.FirstName = request.FirstName;
        citizen.LastName = request.LastName;

        _citizenFileService.SaveAll(citizens);

        _logger.LogInformation("Citizen updated");

        return Ok(citizen);
    }

    [HttpDelete("{ci}")]
    public ActionResult Delete(string ci)
    {
        var citizens = _citizenFileService.GetAll();
        var citizen = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizen == null)
        {
            return NotFound("Citizen not found");
        }

        citizens.Remove(citizen);
        _citizenFileService.SaveAll(citizens);

        _logger.LogInformation("Citizen deleted");

        return Ok("Citizen deleted");
    }
}