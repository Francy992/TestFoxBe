using Database.Core;
using Database.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Dtos;

namespace TestFoxBe.Controllers;
// TODO: Remove it, not required in the final version
[ApiController]
[Route("api/v1/rooms")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Rooms API")]
public class RoomController : ControllerBase
{
    private readonly IUnitOfWorkApi _unitOfWork;

    public RoomController(IUnitOfWorkApi unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get details of one room
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("{id:required:long}", Name = "ROOM-01")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoom(long id)
    {
        var room = await _unitOfWork.RoomRepository.GetById(id);
        return room == null ? NotFound() : Ok(room.Adapt<RoomDto>());
    }

    /// <summary>
    /// Add new room to one room
    /// </summary>
    /// <returns>Element</returns>
    [HttpPost("", Name = "ROOM-02")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoom([FromBody] RoomAddOrUpdDto room)
    {
        var accomodation = await _unitOfWork.AccomodationRepository.GetById(room.AccomodationId);
        if(accomodation == null) return BadRequest(new ErrorDto() { Message = "Accomodation not found" });
        
        var roomType = await _unitOfWork.RoomTypeRepository.GetById(room.RoomTypeId);
        if(roomType == null) return BadRequest(new ErrorDto() { Message = "RoomType not found" });
        
        var roomToAdd = room.Adapt<Room>();
        await _unitOfWork.RoomRepository.Insert(roomToAdd);
        await _unitOfWork.SaveChanges();
        
        return Ok(GetRoomDto(roomToAdd, accomodation, roomType));
    }
    
    /// <summary>
    /// Update details of one room
    /// </summary>
    /// <returns>Element</returns>
    [HttpPut("{id:required:long}", Name = "ROOM-03")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoom(long id, [FromBody] RoomAddOrUpdDto room)
    {
        var accomodation = await _unitOfWork.AccomodationRepository.GetById(room.AccomodationId);
        if(accomodation == null) return BadRequest(new ErrorDto() { Message = "Accomodation not found" });
        
        var roomType = await _unitOfWork.RoomTypeRepository.GetById(room.RoomTypeId);
        if(roomType == null) return BadRequest(new ErrorDto() { Message = "RoomType not found" });
        
        var roomToUpdate = await _unitOfWork.RoomRepository.GetById(id);
        if(roomToUpdate == null) return NotFound();
        
        roomToUpdate.Name = accomodation.Name;
        roomToUpdate.AccomodationId = accomodation.Id;
        roomToUpdate.RoomTypeId = roomType.Id;
        _unitOfWork.RoomRepository.Update(roomToUpdate);
        await _unitOfWork.SaveChanges();
        
        return Ok(GetRoomDto(roomToUpdate, accomodation, roomType));
    }
    
    /// <summary>
    /// Delete room
    /// </summary>
    /// <returns>Element</returns>
    [HttpDelete("{id:required:long}", Name = "ROOM-04")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRoom(long id)
    {
        var roomToDelete = await _unitOfWork.RoomRepository.GetById(id);
        if(roomToDelete == null) return NotFound();
        
        // Delete Room
        _unitOfWork.RoomRepository.Delete(roomToDelete);
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