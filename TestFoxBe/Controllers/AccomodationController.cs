using Database.Core;
using Database.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Dtos;

namespace TestFoxBe.Controllers;

[ApiController]
[Route("api/v1/accomodations")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Accomodations API")]
public class AccomodationController : ControllerBase
{
    private readonly IUnitOfWorkApi _unitOfWork;

    public AccomodationController(IUnitOfWorkApi unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get details of one accomodation
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("{id:required:long}", Name = "ACC-01")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AccomodationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAccomodationById(long id)
    {
        var accomodation = await _unitOfWork.AccomodationRepository.GetById(id);
        return accomodation == null ? NotFound() : Ok(accomodation.Adapt<AccomodationDto>());
    }

    /// <summary>
    /// Get all accomodations
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("", Name = "ACC-05")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AccomodationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAccomodationList()
    {
        var priceList = await _unitOfWork.AccomodationRepository.FindAll();
        return priceList == null ? NotFound() : Ok(priceList.Adapt<List<AccomodationDto>>());
    }

    /// <summary>
    /// Add new accomodation
    /// </summary>
    /// <returns>Element</returns>
    [HttpPost("", Name = "ACC-02")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AccomodationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAccomodation([FromBody] AccomodationAddOrUpdDto accomodation)
    {
        var accomodationToAdd = accomodation.Adapt<Accomodation>();
        await _unitOfWork.AccomodationRepository.Insert(accomodationToAdd);
        await _unitOfWork.SaveChanges();
        return Ok(accomodationToAdd.Adapt<AccomodationDto>());
    }
    
    /// <summary>
    /// Update details of one accomodation
    /// </summary>
    /// <returns>Element</returns>
    [HttpPut("{id:required:long}", Name = "ACC-03")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAccomodation(long id, [FromBody] AccomodationAddOrUpdDto accomodation)
    {
        var accomodationToUpd = await _unitOfWork.AccomodationRepository.GetById(id);
        if(accomodationToUpd == null)
            return NotFound();
        accomodationToUpd.Name = accomodation.Name;
        accomodationToUpd.Address = accomodation.Address;
        _unitOfWork.AccomodationRepository.Update(accomodationToUpd);
        await _unitOfWork.SaveChanges();
        return Ok();
    }
    
    /// <summary>
    /// Delete accomodation
    /// </summary>
    /// <returns>Element</returns>
    [HttpDelete("{id:required:long}", Name = "ACC-04")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccomodation(long id)
    {
        var accomodationToDelete = await _unitOfWork.AccomodationRepository.GetById(id);
        if(accomodationToDelete == null)
            return NotFound();
        
        // Delete Accomodation
        _unitOfWork.AccomodationRepository.Delete(accomodationToDelete);
        await _unitOfWork.SaveChanges();
        
        return Ok();
    }
}