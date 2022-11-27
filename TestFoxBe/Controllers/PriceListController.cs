using Database.Core;
using Database.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Dtos;
using TestFoxBe.Mediators;

namespace TestFoxBe.Controllers;

[ApiController]
[Route("api/v1/price")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Price List API")]
public class PriceListController : ControllerBase
{
    private readonly IUnitOfWorkApi _unitOfWork;
    private readonly INotifierMediatorService _mediator;

    public PriceListController(IUnitOfWorkApi unitOfWork, INotifierMediatorService mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    /// <summary>
    /// Get details of one price list
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("{priceListId:required:long}", Name = "PRL-01")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPriceListById(long priceListId)
    {
        var priceList = await _unitOfWork.PriceListRepository.GetById(priceListId);
        return priceList == null ? NotFound() : Ok(priceList.Adapt<PriceListDto>());
    }


    /// <summary>
    /// Get all price list
    /// </summary>
    /// <returns>Element</returns>
    [HttpGet("", Name = "PRL-05")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PriceListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPriceList()
    {
        var priceList = await _unitOfWork.PriceListRepository.FindAll();
        return priceList == null ? NotFound() : Ok(priceList.Adapt<List<PriceListDto>>());
    }

    /// <summary>
    /// Add new price for date
    /// </summary>
    /// <returns>Element</returns>
    [HttpPost("", Name = "PRL-02")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PriceListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoomType([FromBody] PriceListAddOrUpdateDto price)
    {
        if(price.Price is <= 0 or > 1000)
            return BadRequest("Price must be between 1 and 1000");
        
        var roomType = await _unitOfWork.RoomTypeRepository.GetById(price.RoomTypeId);
        if (roomType == null)
            return NotFound();
        
        var canUpdate = await CanInsertPriceForCurrentRoomTypeAndDate(price, roomType);
        if (!canUpdate)
            return BadRequest("Price must be higher than price of room type increment");
        
        var priceListToAdd = price.Adapt<PriceList>();
        priceListToAdd.Date = priceListToAdd.Date.Date;
        await _unitOfWork.PriceListRepository.Insert(priceListToAdd);
        await _mediator.Notify(NotificationTypeEnum.UpdatePriceConnectedRoomType, new UpdatePriceConnectedRoomTypeDto() { RoomTypeId = roomType.Id, Price = price.Price, Date = priceListToAdd.Date});
        // await UpdateOtherPriceListConnectedWithCurrentRoomType(priceListToAdd);
        await _unitOfWork.SaveChanges();
        
        return Ok(priceListToAdd.Adapt<PriceListDto>());
    }

    /// <summary>
    /// Update price list
    /// </summary>
    /// <returns>Element</returns>
    [HttpPut("{priceListId:required:long}", Name = "PRL-03")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoomType(long priceListId, [FromBody] PriceListAddOrUpdateDto price)
    {
        if(price.Price is <= 0 or > 1000)
            return BadRequest("Price must be between 1 and 1000");
        
        var roomType = await _unitOfWork.RoomTypeRepository.GetById(price.RoomTypeId);
        if (roomType == null)
            return NotFound();
        
        var priceListToUpd = await _unitOfWork.PriceListRepository.GetById(priceListId);
        if (priceListToUpd == null)
            return NotFound();
        
        var canUpdate = await CanInsertPriceForCurrentRoomTypeAndDate(price, roomType);
        if (!canUpdate)
            return BadRequest("Price must be higher than price of room type increment");
        
        priceListToUpd.Price = price.Price;
        priceListToUpd.Date = price.Date.Date;
        priceListToUpd.RoomTypeId = price.RoomTypeId;
        _unitOfWork.PriceListRepository.Update(priceListToUpd);

        await _mediator.Notify(NotificationTypeEnum.UpdatePriceConnectedRoomType, new UpdatePriceConnectedRoomTypeDto() { RoomTypeId = roomType.Id, Price = price.Price, Date = priceListToUpd.Date});
        await _unitOfWork.SaveChanges();
        
        return Ok();
    }
    
    /// <summary>
    /// Delete room type
    /// </summary>
    /// <returns>Element</returns>
    [HttpDelete("{priceListId:required:long}", Name = "PRL-04")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePriceList(long priceListId)
    {
        var priceList = await _unitOfWork.PriceListRepository.GetById(priceListId);
        if (priceList == null)
            return NotFound();
        _unitOfWork.PriceListRepository.Delete(priceList);
        await _unitOfWork.SaveChanges();
        return Ok();
    }

    #region SupportMethods
    private async Task<bool> CanInsertPriceForCurrentRoomTypeAndDate(PriceListAddOrUpdateDto price, RoomType roomType)
    {
        if (roomType.RoomTypeIncrementId.HasValue)
        {
            // Check if I can create price list for this room type
            var priceLists = await _unitOfWork.PriceListRepository.GetByRoomTypeAndDateAsync(roomType.RoomTypeIncrementId.Value, price.Date);
            foreach (var priceList in priceLists)
            {
                if (price.Price < priceList.Price + (priceList.Price * roomType.PriceIncrementPercentage.Value / 100))
                    return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Check, if there are other price list connected with current room type and update them
    /// For do this, I check if there are other room type connected with current room type and I update their price list
    /// I use queue to simulate recursion
    /// </summary>
    /// <param name="priceListToAdd"></param>
    private async Task UpdateOtherPriceListConnectedWithCurrentRoomType(PriceList priceListToAdd)
    {
        var roomTypeQueue = new Queue<long>();
        roomTypeQueue.Enqueue(priceListToAdd.RoomTypeId);
        do
        {
            var currentRoomTypeId = roomTypeQueue.Dequeue();
            var roomTypeToCheck = await _unitOfWork.RoomTypeRepository.FindByRoomTypeIncrementIdAsync(currentRoomTypeId);
            foreach(var roomTypeToCheckIncrement in roomTypeToCheck)
            {
                var priceListToCheckIncrement = await _unitOfWork.PriceListRepository.GetByRoomTypeAndDateAsync(roomTypeToCheckIncrement.Id, priceListToAdd.Date);
                foreach(var priceListToIncrement in priceListToCheckIncrement)
                {
                    var minPrice = priceListToAdd.Price + (priceListToAdd.Price * roomTypeToCheckIncrement.PriceIncrementPercentage!.Value / 100);
                    if (priceListToIncrement.Price < minPrice)
                    {
                        priceListToIncrement.Price = minPrice;
                        _unitOfWork.PriceListRepository.Update(priceListToIncrement);
                        if(!roomTypeQueue.Contains(priceListToIncrement.RoomTypeId))
                            roomTypeQueue.Enqueue(priceListToIncrement.RoomTypeId);
                    }
                }
            }
        }
        while(roomTypeQueue.Count > 0);
    }
    #endregion
}