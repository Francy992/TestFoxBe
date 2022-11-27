using Microsoft.AspNetCore.Mvc;
using TestFoxBe.Controllers;
using TestFoxBe.Dtos;
using TestFoxBe.Mocks;
using Xunit;

namespace UnitTest;

public class AccomodationControllerTest : BaseTest
{
    private async Task<AccomodationController> GetController()
    {
        var controller =  new AccomodationController(UnitOfWorkApi);
        return controller;
    }
    
    [Fact]
    public async Task GetAccomodationById_Ok()
    {
        var controller = await GetController();
        var element = DbMock.Accomodations.First();
        var result = await controller.GetAccomodationById(element.Id);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as AccomodationDto;
        Assert.NotNull(dto);
        Assert.Equal(dto.Id, element.Id); 
    }
    
    [Fact]
    public async Task GetAccomodationList_Ok()
    {
        var controller = await GetController();
        var result = await controller.GetAccomodationList();
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as List<AccomodationDto>;
        Assert.NotNull(dto);
        Assert.Equal(dto.Count, DbMock.Accomodations.Count); 
    }
    
    [Fact]
    public async Task AddAccomodation_Ok()
    {
        var controller = await GetController();
        var model = new AccomodationAddOrUpdDto()
        {
            Name = "Test",
            Address = "Via Test",
        };
        var result = await controller.AddAccomodation(model);
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var dto = ((ObjectResult)result).Value as AccomodationDto;
        Assert.NotNull(dto);
        Assert.Equal(dto.Address, model.Address); 
        Assert.Equal(dto.Name, model.Name);

        var totalElement = await UnitOfWorkApi.AccomodationRepository.FindAll();
        Assert.Equal(totalElement.Count(), DbMock.Accomodations.Count + 1);
    }

    [Fact]
    public async Task UpdateAccomodation_Ko_AccomodationToUpdNotFound()
    {
        var controller = await GetController();
        var model = new AccomodationAddOrUpdDto()
        {
            Name = "Test",
            Address = "Via Test",
        };
        var result = await controller.UpdateAccomodation(0, model);
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAccomodation_Ok()
    {
        var controller = await GetController();
        var element = DbMock.Accomodations.First();
        var model = new AccomodationAddOrUpdDto()
        {
            Name = "Test",
            Address = "Via Test + 1000",
        };
        var result = await controller.UpdateAccomodation(element.Id, model);
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        
        var dto = await UnitOfWorkApi.AccomodationRepository.GetById(element.Id);
        Assert.Equal(dto.Address, model.Address);
        Assert.Equal(dto.Name, model.Name);
    }
    

    [Fact]
    public async Task DeleteAccomodation_Ko_AccomodationToDeleteNotFound()
    {
        var controller = await GetController();
        var result = await controller.DeleteAccomodation(0);
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }
    

    [Fact]
    public async Task DeleteAccomodation_Ok()
    {
        var controller = await GetController();
        var element = DbMock.Accomodations.First();
        var result = await controller.DeleteAccomodation(element.Id);
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        var totalElementOnDb = await UnitOfWorkApi.AccomodationRepository.FindAll();
        Assert.Equal(totalElementOnDb.Count(), DbMock.Accomodations.Count - 1);
    }

}