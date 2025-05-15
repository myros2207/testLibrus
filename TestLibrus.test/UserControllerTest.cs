using TestLibrus.Controllers;
using TestLibrus.Controllers.Requests;
using TestLibrus.Controllers.Responses;
using TestLibrus.Data;
using TestLibrus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestLibrus.test;

public class UserControllerTests
{
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
    public void CreateUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new UserController(context);
        var input = new UserReq_create
        {
            name = "Test User"
        };

        // Act
        var result = controller.CreateUser(input);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdUser = Assert.IsType<UserRes_create>(actionResult.Value);
        Assert.Equal("Test User", createdUser.Name);
    }

    [Fact]
    public void GetUser_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var User = new User { Name = "Test User" };
        context.Users.Add(User);
        context.SaveChanges();

        var controller = new UserController(context);

        // Act
        var result = controller.GetUser(User.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<UserRes_get>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal("Test User", result.Value.Name);
    }

    [Fact]
    public void UpdateUser_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var User = new User { Name = "Old Name" };
        context.Users.Add(User);
        context.SaveChanges();

        var controller = new UserController(context);
        var input = new UserReq_create
        {
            name = "Updated Name"
        };

        // Act
        var result = controller.UpdateUser(User.Id, input);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var updatedUser = Assert.IsType<User>(actionResult.Value);
        Assert.Equal("Updated Name", updatedUser.Name);
    }

    [Fact]
    public void DeleteUser_ShouldReturnTrue_WhenUserHasNoItems()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var User = new User { Name = "Test User" };
        context.Users.Add(User);
        context.SaveChanges();

        var controller = new UserController(context);

        // Act
        var result = controller.DeleteUser(User.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(result.Value);
    }

    [Fact]
    public void DeleteUser_ShouldReturnFalse_WhenUserHasItems()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var User = new User { Name = "Test User", Items = new List<Item> { new Item { Name = "Test Item", Description = "Lorem Ipsum Dolor Sit"} } };
        context.Users.Add(User);
        context.SaveChanges();

        var controller = new UserController(context);

        // Act
        var result = controller.DeleteUser(User.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.False(result.Value);
    }
}