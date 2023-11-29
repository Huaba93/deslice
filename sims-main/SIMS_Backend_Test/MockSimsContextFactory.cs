using Microsoft.EntityFrameworkCore;
using SIMS_Backend;

namespace Test2;

public class MockSimsContextFactory : ISIMSContextFactory
{
    public SIMSContext Create()
    {
        var dbName = Guid.NewGuid().ToString(); // Erzeugt einen eindeutigen Namen für die Datenbank
        var options = new DbContextOptionsBuilder<SIMSContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var dbContext = new SIMSContext(options);

        // Vorbefüllen der Datenbank mit Employees
        SIMSContextSeeder.SeedEmployees(dbContext);

        return dbContext;
    }
}