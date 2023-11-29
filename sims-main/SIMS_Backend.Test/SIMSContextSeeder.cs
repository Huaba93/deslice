namespace SIMS_Backend.Test;

public static class SIMSContextSeeder
{
    public static void SeedEmployees(SIMSContext context)
    {
        var employees = new List<Employee>();

        for (int i = 1; i <= 20; i++)
        {
            employees.Add(new Employee
            {
                EmployeeID = i,
                UID = $"UID{i}",
                FirstName = $"Vorname{i}",
                LastName = $"Nachname{i}",
                Mail = $"employee{i}@example.com",
                Phone = $"123-456-789{i}",
                SuperiorID = i % 5 == 0 ? (int?) null : i // Beispiel für eine Bedingung
            });
        }

        context.Employees.AddRange(employees);
        context.SaveChanges();
    }
}