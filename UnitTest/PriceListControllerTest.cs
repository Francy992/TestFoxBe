using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestFoxBe.Controllers;
using TestFoxBe.Dtos;
using TestFoxBe.Mediators;
using TestFoxBe.Mocks;
using Xunit;

namespace UnitTest;

public class PriceListControllerTest : BaseTest
{
    private Mock<INotifierMediatorService> _notifierMediatorServiceMock = GetNotifierMediatorServiceMock();
    private async Task<PriceListController> GetController()
    {
        var controller =  new PriceListController(UnitOfWorkApi, _notifierMediatorServiceMock.Object);
        return controller;
    }
    
    [Fact]
    public async Task GetPriceListById_Ok()
    {
        var controller = await GetController();
        var element = DbMock.RoomTypes.First();
        var result = await controller.GetPriceListById(element.Id);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as PriceListDto;
        Assert.NotNull(dto);
        Assert.Equal(dto.Id, element.Id); 
    }
    
    [Fact]
    public async Task GetPriceListList_Ok()
    {
        var controller = await GetController();
        var result = await controller.GetPriceList();
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as List<PriceListDto>;
        Assert.NotNull(dto);
        Assert.Equal(dto.Count, DbMock.PriceLists.Count); 
    }

    [Fact]
    public async Task AddElement_Ko_PriceLessZero()
    {
        var controller = await GetController();
        var roomTypeId = DbMock.RoomTypes.First().Id;
        var model = new PriceListAddOrUpdateDto()
        {
            RoomTypeId = roomTypeId,
            Price = -1,
            Date = DateTime.Now
        };
        var result = await controller.AddPriceList(model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddElement_Ko_RoomTypeNotFound()
    {
        var controller = await GetController();
        var roomTypeId = DbMock.RoomTypes.First().Id;
        var model = new PriceListAddOrUpdateDto()
        {
            RoomTypeId = roomTypeId + 1000,
            Price = 1,
            Date = DateTime.Now
        };
        var result = await controller.AddPriceList(model);
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task AddElement_Ko_CantInsertForMinimumPrice()
    {
        var controller = await GetController();
        var roomType = DbMock.RoomTypes.First(x => x.RoomTypeIncrementId.HasValue);
        var wrongPrice = DbMock.PriceLists.First(x => x.RoomTypeId == roomType.Id);
        var model = new PriceListAddOrUpdateDto()
        {
            RoomTypeId = roomType.Id,
            Price = wrongPrice.Price - 1,
            Date = wrongPrice.Date
        };
        var result = await controller.AddPriceList(model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task AddElement_Ok()
    {
        var controller = await GetController();
        var roomType = DbMock.RoomTypes.First(x => x.RoomTypeIncrementId.HasValue);
        var wrongPrice = DbMock.PriceLists.First(x => x.RoomTypeId == roomType.Id);
        var model = new PriceListAddOrUpdateDto()
        {
            RoomTypeId = roomType.Id,
            Price = wrongPrice.Price + 1,
            Date = wrongPrice.Date
        };
        var result = await controller.AddPriceList(model);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

}