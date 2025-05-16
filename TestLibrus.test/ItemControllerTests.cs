using TestLibrus.Controllers;
using TestLibrus.Controllers.Requests;
using TestLibrus.Controllers.Responses;
using TestLibrus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestLibrus.Models;
using Xunit;

namespace TestLibrus.test;

public class ItemControllerTests : IAsyncLifetime
{
    public Task InitializeAsync()
    {
        var context = GetInMemoryDbContext();
        
        User testUser = new User
        {
            Id = 1,
            Name = "Tung tung tung Sahur",
            BirthDate = DateTime.Now,
            DeathDate = DateTime.Now,
        };

        Item testItem = new Item
        {
            Id = 1,
            Name = "Test Item",
            Description = "Test Description",
            UserId = testUser.Id
        };

        context.Add(testItem);
        context.Add(testUser);
        context.SaveChanges();
        
        return Task.CompletedTask;
    }
    
    public Task DisposeAsync()
    {
        GetInMemoryDbContext().Database.EnsureDeletedAsync();

        return Task.CompletedTask;
    }
    
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.EnsureCreated();
        return context;
    }
    
    [Fact]
    public void CreateItem_ShouldReturnCreatedItem()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ItemController(context);
        var input = new itemReq_create
        {
            Name = "Test Item",
            Description = "Test Description",
            UserId = 1
        };

        // Act
        var result = controller.CreateItem(input);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdItem = Assert.IsType<itemRes_get>(actionResult.Value);
        Assert.Equal("Test Item", createdItem.Name);
    }

    [Fact]
    public void GetItem_ShouldReturnItem_WhenItemExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ItemController(context);

        // Act
        var result = controller.GetItem(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<itemRes_get>>(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void DeleteItem_ShouldReturnTrue_WhenItemExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new ItemController(context);

        // Act
        var result = controller.DeleteItem(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(result.Value);
    }
}