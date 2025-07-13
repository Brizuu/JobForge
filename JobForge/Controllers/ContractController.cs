using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobForge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmploymentContractsController : ControllerBase
{
    private readonly IContractService _service;

    public EmploymentContractsController(IContractService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin, Company, PublicFacility")]
    [HttpPost("company/create")]
    public async Task<IActionResult> AddContract([FromBody] EmploymentContractDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        await _service.AddEmploymentContractAsync(userId, dto);
        return Ok();
    }
    
    [Authorize(Roles = "Admin, Company, PublicFacility")]
    [HttpDelete("company/{contractId:int}")]
    public async Task<IActionResult> DeleteContract(int contractId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        var success = await _service.DeleteEmploymentContractAsync(userId, contractId);
        if (!success)
        {
            return NotFound("Contract not found or you are not authorized to delete it.");
        }

        return NoContent();
    }
    
    [Authorize(Roles = "Admin, Premium")]
    [HttpPost("user/{contractId:int}/accept")]
    public async Task<IActionResult> AcceptContract(int contractId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var contractorId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        var success = await _service.UpdateContractStatusAsync(contractId, contractorId, "Accepted");
        if (!success)
        {
            return BadRequest("Contract not found or already processed.");
        }

        return Ok("Contract accepted.");
    }

    [Authorize(Roles = "Admin, Premium")]
    [HttpPost("user/{contractId:int}/reject")]
    public async Task<IActionResult> RejectContract(int contractId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var contractorId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        var success = await _service.UpdateContractStatusAsync(contractId, contractorId, "Rejected");
        if (!success)
        {
            return BadRequest("Contract not found or already processed.");
        }

        return Ok("Contract rejected.");
    }

    [Authorize(Roles = "Admin, Premium, Company, PublicFacility")]
    [HttpGet("get-contract/{contractId:int?}")]
    [Authorize]
    public async Task<IActionResult> GetContracts(int? contractId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        if (contractId.HasValue)
        {
            var contract = await _service.GetContractByIdAsync(contractId.Value);

            if (contract == null)
                return NotFound("Contract not found");

            if (contract.UserId != userId && contract.Contractor != userId)
                return Forbid("You do not have access to this contract.");

            return Ok(contract);
        }

        var userContracts = await _service.GetUserContractsAsync(userId);
        return Ok(userContracts);
    }
    
    [Authorize(Roles = "Admin, Premium, Company, PublicFacility")]
    [HttpGet("get-contracts")]
    public async Task<IActionResult> GetMyContracts()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var contracts = await _service.GetUserContractsAsync(userId);
        return Ok(contracts);
    }


}
