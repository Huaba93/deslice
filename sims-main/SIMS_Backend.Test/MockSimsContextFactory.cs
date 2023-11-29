using Microsoft.EntityFrameworkCore;

namespace SIMS_Backend.Test;

public class MockSimsContextFactory : ISIMSContextFactory
{
    public SIMSContext Create()
    {
        var options = new DbContextOptionsBuilder<SIMSContext>()
            .UseInMemoryDatabase(databaseName: "InMemorySIMSDb")
            .Options;

        var dbContext = new SIMSContext(options);

        // Vorbefüllen der Datenbank mit Employees
        SIMSContextSeeder.SeedEmployees(dbContext);

        return dbContext;
    }
}