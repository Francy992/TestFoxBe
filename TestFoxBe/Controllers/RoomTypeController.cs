using Database.Core;
using Database.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Dtos;
using TestFoxBe.Mediators;

namespace TestFoxBe.Controllers;

[ApiController]
[Route("api/v1/roomtypes")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Room Types API")]
public class RoomTypeController : ControllerBase
{
    private readonly IUnitOfWorkApi _unitOfWork;
    private readonly INotifierMediatorService _notifierMediatorService;

    public RoomTypeController(IUnitOfWorkApi unitOfWork, INotifierMediatorService notifierMediatorService)
    {
        _unitOfWork = unitOfWork;
        _notifierMediatorService = notifierMediatorService;
    }

    /// <summary>
    /// Get details of one room type
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("{id:required:long}", Name = "RMT-01")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoomTypeById(long id)
    {
        var room = await _unitOfWork.RoomTypeRepository.GetById(id);
        return room == null ? NotFound() : Ok(room.Adapt<RoomTypeDto>());
    }

    /// <summary>
    /// Get all room type list
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("", Name = "RMT-05")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<RoomTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoomTypeList()
    {
        var priceList = await _unitOfWork.RoomTypeRepository.FindAll();
        return priceList == null ? NotFound() : Ok(priceList.Adapt<List<RoomTypeDto>>());
    }
    
    /// <summary>
    /// Add new room to one room type
    /// </summary>
    /// <returns>Element</returns>
    [HttpPost("", Name = "RMT-02")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoomType([FromBody] RoomTypeAddOrUpdDto room)
    {
        RoomType roomTypeToIncrement = null;
        if (room.RoomTypeIncrementId.HasValue)
        {
            roomTypeToIncrement = await _unitOfWork.RoomTypeRepository.GetById(room.RoomTypeIncrementId.Value);
            if(roomTypeToIncrement == null)
                return BadRequest("Room Type not found");
        }
        
        if(room.RoomTypeIncrementId.HasValue && !room.PriceIncrementPercentage.HasValue)
            return BadRequest("Increment percentage needed if exist room type increment id.");

        var roomTypeToAdd = room.Adapt<RoomType>();
        await _unitOfWork.RoomTypeRepository.Insert(roomTypeToAdd);
        await _unitOfWork.SaveChanges();
        
        var result = roomTypeToAdd.Adapt<RoomTypeDto>();
        result.RoomTypeIncrement = roomTypeToIncrement?.Adapt<RoomTypeDto>();
        return Ok(result);
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
        
        RoomType roomTypeToIncrement = null;
        if (room.RoomTypeIncrementId.HasValue)
        {
            roomTypeToIncrement = await _unitOfWork.RoomTypeRepository.GetById(room.RoomTypeIncrementId.Value);
            if(roomTypeToIncrement == null)
                return BadRequest("Room Type not found");
        }
        
        if(room.RoomTypeIncrementId.HasValue && !room.PriceIncrementPercentage.HasValue)
            return BadRequest("Increment percentage needed if exist room type increment id.");
        
        if(room.RoomTypeIncrementId.HasValue && room.RoomTypeIncrementId.Value == id)
            return BadRequest("Room type increment id can't be the same as room type id.");

        roomTypeToUpdate.Name = room.Name;
        roomTypeToUpdate.PriceIncrementPercentage = room.PriceIncrementPercentage;
        roomTypeToUpdate.RoomTypeIncrementId = room.RoomTypeIncrementId;
        _unitOfWork.RoomTypeRepository.Update(roomTypeToUpdate);
        await _unitOfWork.SaveChanges();

        if(room.RoomTypeIncrementId.HasValue)
            await _notifierMediatorService.Notify(NotificationTypeEnum.UpdatePriceConnectedRoomType, new UpdatePriceConnectedRoomTypeDto { RoomTypeId = roomTypeToUpdate.RoomTypeIncrementId.Value });
        
        return Ok();
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
        if(existsPriceList) return BadRequest("Operation not allowed. Remove price list with current roomType");
        
        var existsRoomTypeConnected = await _unitOfWork.RoomTypeRepository.ExistsByRoomTypeIdAsync(id);
        if(existsRoomTypeConnected) return BadRequest("Operation not allowed. Remove room type connected to current");
        
        // Delete RoomType
        _unitOfWork.RoomTypeRepository.Delete(roomToDelete);
        await _unitOfWork.SaveChanges();
        
        return Ok();
    }
    
}