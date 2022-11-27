using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestFoxBe.Controllers;
using TestFoxBe.Dtos;
using TestFoxBe.Mediators;
using TestFoxBe.Mocks;
using Xunit;

namespace UnitTest;

public class RoomTypeControllerTest : BaseTest
{
    private Mock<INotifierMediatorService> _notifierMediatorServiceMock = GetNotifierMediatorServiceMock();
    private async Task<RoomTypeController> GetController()
    {
        var controller =  new RoomTypeController(UnitOfWorkApi, _notifierMediatorServiceMock.Object);
        return controller;
    }
    
    [Fact]
    public async Task GetRoomTypeById_Ok()
    {
        var controller = await GetController();
        var element = DbMock.RoomTypes.First();
        var result = await controller.GetRoomTypeById(element.Id);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as RoomTypeDto;
        Assert.NotNull(dto);
        Assert.Equal(dto.Id, element.Id); 
    }
    
    [Fact]
    public async Task GetRoomTypeList_Ok()
    {
        var controller = await GetController();
        var result = await controller.GetRoomTypeList();
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as List<RoomTypeDto>;
        Assert.NotNull(dto);
        Assert.Equal(dto.Count, DbMock.RoomTypes.Count); 
    }

    [Fact]
    public async Task AddElement_Ko_RoomTypeNotFound()
    {
        var controller = await GetController();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = 0,
        };
        var result = await controller.AddRoomType(model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddElement_Ko_RoomTypeIncrementPercentageNotFound()
    {
        var controller = await GetController();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = DbMock.RoomTypes.First().Id,
        };
        var result = await controller.AddRoomType(model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddElement_Ok()
    {
        var controller = await GetController();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = DbMock.RoomTypes.First().Id,
            PriceIncrementPercentage = 10,
        };
        var result = await controller.AddRoomType(model);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        
        var dto = ((ObjectResult)result).Value as RoomTypeDto;
        Assert.NotNull(dto);
        Assert.Equal(dto.Name, model.Name);
        Assert.Equal(dto.RoomTypeIncrement.Id, model.RoomTypeIncrementId);
        Assert.Equal(dto.PriceIncrementPercentage, model.PriceIncrementPercentage);
    }
    
    [Fact]
    public async Task UpdateElement_Ko_RoomTypeIncrementNotFound()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = 0,
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id, model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateElement_Ko_RoomTypeNotFound()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = 0,
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id + 10000, model);
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateElement_Ko_RoomTypeIncrementPercentageNotFound()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = DbMock.RoomTypes.First().Id,
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id, model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateElement_Ko_RoomTypeIncrementIdSameOfCurrentId()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = elementToUpd.Id,
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id, model);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateElement_Ok()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test",
            RoomTypeIncrementId = DbMock.RoomTypes.First(x => x.Id != elementToUpd.Id).Id,
            PriceIncrementPercentage = 10,
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id, model);
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        
        var dto = await UnitOfWorkApi.RoomTypeRepository.GetById(elementToUpd.Id);
        Assert.Equal(dto.Name, model.Name);
        Assert.Equal(dto.RoomTypeIncrement.Id, model.RoomTypeIncrementId);
        Assert.Equal(dto.PriceIncrementPercentage, model.PriceIncrementPercentage);
        Assert.Equal(1, _notifierMediatorServiceMock.Invocations.Count);
    }

    [Fact]
    public async Task UpdateElement_Ok_WhitoutNotifier()
    {
        var controller = await GetController();
        var elementToUpd = DbMock.RoomTypes.First();
        var model = new RoomTypeAddOrUpdDto()
        {
            Name = "Test22"
        };
        var result = await controller.UpdateRoomType(elementToUpd.Id, model);
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        
        var dto = await UnitOfWorkApi.RoomTypeRepository.GetById(elementToUpd.Id);
        Assert.Equal(dto.Name, model.Name);
        Assert.Equal(null, model.RoomTypeIncrementId);
        Assert.Equal(dto.PriceIncrementPercentage, model.PriceIncrementPercentage);
        Assert.Equal(0, _notifierMediatorServiceMock.Invocations.Count);
    }
    
    [Fact]
    public async Task DeleteRoomTypeById_Ko_PriceListConnectedToCurrentRoomType()
    {
        var controller = await GetController();
        var element = DbMock.PriceLists.First();
        var result = await controller.DeleteRoomType(element.RoomTypeId);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteRoomTypeById_Ko_RoomTypeConnectedToCurrentRoomType()
    {
        var controller = await GetController();
        var element = DbMock.RoomTypes.First(x => x.RoomTypeIncrementId != null);
        var result = await controller.DeleteRoomType(element.RoomTypeIncrementId.Value);
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteRoomTypeById_Ko_IdNotFound()
    {
        var controller = await GetController();
        var element = DbMock.RoomTypes.First(x => x.RoomTypeIncrementId != null);
        var result = await controller.DeleteRoomType(element.Id + 11000);
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task DeleteRoomTypeById_Ok()
    {
        var controller = await GetController();
        var priceListToDelete = (await UnitOfWorkApi.PriceListRepository.FindAll()).Where(x => x.RoomTypeId == 4).ToList();
        UnitOfWorkApi.PriceListRepository.DeleteRange(priceListToDelete);
        await UnitOfWorkApi.SaveChanges();
        DetachAllEntities(DbContextAccomodations);
        
        var element = DbMock.RoomTypes.First(x => x.Id == 4);
        var result = await controller.DeleteRoomType(element.Id);
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);

        var dto = await UnitOfWorkApi.RoomTypeRepository.FindAll();
        Assert.Equal(dto.Count, DbMock.RoomTypes.Count - 1);
    }
}