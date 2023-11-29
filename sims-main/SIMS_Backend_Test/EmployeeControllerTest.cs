using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIMS_Backend;
using SIMS_Backend.Controllers;

namespace Test2;

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

    [Fact]
    public void Get_ReturnsAllEmployees_WhenCalledWithAdminToken()
    {
        // Arrange
        var result = _controller.Get(_mockAuth.AdminToken);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Employee>>(actionResult.Value);
        Assert.Equal(20, returnValue.Count); // Angenommen, Sie haben 20 Mitarbeiter
    }

    [Fact]
    public void CreateEmployee_AddsNewEmployee_WhenEmployeeIsValid()
    {
        var rand = new Random();
        var i = rand.Next(30, 100);
        // Arrange
        var newEmployee = new Employee
        {
            EmployeeID = i,
            UID = $"UID{i}",
            FirstName = $"Vorname{i}",
            LastName = $"Nachname{i}",
            Mail = $"employee{i}@example.com",
            Phone = $"123-456-789{i}",
            SuperiorID = i % 5 == 0 ? (int?) null : i // Beispiel für eine Bedingung
        };

        // Act
        var result = _controller.CreateEmployee(newEmployee);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public void CreateEmployee_AddsNewEmployee_WhenEmployeeIsInValid()
    {
        // Arrange
        var newEmployee = new Employee
        {
            //no properties. Some of them are required
        };

        // Act
        var result = _controller.CreateEmployee(newEmployee);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }


    [Fact]
    public void UpdateEmployee_UpdatesEmployee_WhenEmployeeExists()
    {
        // Arrange
        int existingId = 3; // Angenommen, dieser Mitarbeiter existiert
        var employeeToUpdate = new Employee {EmployeeID = existingId, /* weitere Eigenschaften */};

        // Act
        var result = _controller.UpdateEmployee(existingId, employeeToUpdate);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void DeleteEmployee_DeletesEmployee_WhenIdIsValid()
    {
        // Arrange
        int existingId = 4; // Angenommen, dieser Mitarbeiter existiert

        // Act
        var result = _controller.DeleteEmployee(existingId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Get_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = _mockAuth.InValidToken;

        // Act
        var result = _controller.Get(invalidToken);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetEmployeeById_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        int validId = 5;
        var invalidToken = _mockAuth.InValidToken;

        // Act
        var result = _controller.Get(validId, invalidToken);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
}