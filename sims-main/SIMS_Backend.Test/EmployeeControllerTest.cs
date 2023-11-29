using Microsoft.AspNetCore.Mvc;
using SIMS_Backend.Controllers;

namespace SIMS_Backend.Test;

public class EmployeeControllerTests
{
    private readonly EmployeeController _controller;
    private readonly MockAuthenticator _mockAuth;
    private readonly MockSimsContextFactory _contextFactory;

    public EmployeeControllerTests()
    {
        _mockAuth = new MockAuthenticator();
        _contextFactory = new MockSimsContextFactory();
        _controller = new EmployeeController(_mockAuth, _contextFactory);
    }

    [Fact]
    public void GetEmployeeById_ReturnsEmployee_WhenIdIsValid()
    {
        // Arrange
        int validId = 5; // Angenommen, Mitarbeiter mit dieser ID existiert

        // Act
        var result = _controller.Get(validId, _mockAuth.AdminToken);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Employee>(actionResult.Value);
        Assert.Equal(validId, returnValue.EmployeeID);
    }
}