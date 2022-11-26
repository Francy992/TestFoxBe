using Database.Core;
using Database.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Dtos;

namespace TestFoxBe.Controllers;

[ApiController]
[Route("api/v1/roomtypes")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Price List API")]
public class PriceListController : ControllerBase
{
    private readonly IUnitOfWorkApi _unitOfWork;

    public PriceListController(IUnitOfWorkApi unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get details of one price list
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("{roomTypeId:required:long}/price/{priceListId:required:long}", Name = "PRL-01")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomTypeBaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoom(long roomTypeId, long priceListId)
    {
        var priceList = await _unitOfWork.PriceListRepository.GetById(priceListId);
        return priceList == null || priceList.RoomTypeId != roomTypeId ? NotFound() : Ok(priceList.Adapt<PriceListDto>());
    }

    /// <summary>
    /// Add new price for date
    /// </summary>
    /// <returns>Element</returns>
    [HttpPost("{roomTypeId:required:long}/price", Name = "PRL-02")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomTypeBaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoomType([FromBody] RoomTypeAddOrUpdDto room)
    {
        var roomTypeToAdd = room.Adapt<RoomType>();
        await _unitOfWork.RoomTypeRepository.Insert(roomTypeToAdd);
        await _unitOfWork.SaveChanges();
        
        return Ok(roomTypeToAdd.Adapt<RoomTypeBaseDto>());
    }
    
    /// <summary>
    /// Update details of one room type
    /// </summary>
    /// <returns>Element</returns>
    [HttpPut("{id:required:long}", Name = "RMT-03")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoomType(long id, [FromBody] RoomTypeAddOrUpdDto room)
    {
        var roomTypeToUpdate = await _unitOfWork.RoomTypeRepository.GetById(id);
        if (roomTypeToUpdate == null) 
            return NotFound();
        
        roomTypeToUpdate.Name = room.Name;
        _unitOfWork.RoomTypeRepository.Update(roomTypeToUpdate);
        await _unitOfWork.SaveChanges();
        
        return Ok(roomTypeToUpdate.Adapt<RoomTypeBaseDto>());
    }
    
    /// <summary>
    /// Delete room type
    /// </summary>
    /// <returns>Element</returns>
    [HttpDelete("{id:required:long}", Name = "RMT-04")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRoomType(long id)
    {
        var roomToDelete = await _unitOfWork.RoomTypeRepository.GetById(id);
        if(roomToDelete == null) return NotFound();
        
        var existsPriceList = await _unitOfWork.PriceListRepository.ExistsByRoomTypeIdAsync(id);
        if(existsPriceList) return BadRequest(new ErrorDto() { Message = "Operation not allowed. Remove price list with current roomType" });
        
        var existsRoom = await _unitOfWork.RoomRepository.ExistsByRoomTypeIdAsync(id);
        if(existsRoom) return BadRequest(new ErrorDto() { Message = "Operation not allowed. Remove room with current roomType" });
        
        // Delete Room
        _unitOfWork.RoomTypeRepository.Delete(roomToDelete);
        await _unitOfWork.SaveChanges();
        
        return Ok();
    }

    #region SupportMethods
    private RoomDto GetRoomDto(Room room, Accomodation accomodation, RoomType roomType)
    {
        var result = room.Adapt<RoomDto>();
        result.Accomodation = accomodation.Adapt<AccomodationDto>();
        result.RoomType = roomType.Adapt<RoomTypeBaseDto>();
        return result;
    }
    #endregion
}