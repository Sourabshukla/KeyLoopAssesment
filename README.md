# KeyLoopAssesment
**Keyloop Technical Assessment | Scenario C — Demand**

ASP.NET Core 10 Web API using Clean Architecture, Entity Framework Core and SQL Server LocalDB.

## Prerequisites

- .NET 10 SDK
- Visual Studio with ASP.NET Core development support
- SQL Server Express LocalDB

## How to Run

1. Clone or download the repository.
2. Open `KeyLoopSalesman.slnx` in Visual Studio.
3. Set `KeyLoopSalesman.Api` as the **Startup Project**.
4. Verify the connection string in the API configuration:

   ```json
   {
     "ConnectionStrings": {
       "LeadManagementDb": "Server=(localdb)\\MSSQLLocalDB;Database=KeyLoopSalesmanDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

5. Select the **Development** environment.
6. Build the solution and press **F5** to run.

**Database setup is automatic:** On Development startup, the application applies existing EF Core migrations and runs `DbSeeder` to insert sample leads if the database is empty. No manual migration or seeding command is needed.

## Swagger API

Open:

**https://localhost:7005/swagger**

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/leads` | View all leads |
| GET | `/api/leads/{id}` | View lead details and history |
| GET | `/api/leads/{id}/activities` | View follow-up activities |
| POST | `/api/leads/{id}/activities` | Create a follow-up |

Example POST request:

```json
{
  "activityType": "Call",
  "notes": "Customer requested a test drive."
}
```

After creating an activity, execute `GET /api/leads/1/activities` to verify it was saved.

## Run Tests

In Visual Studio, open **Test → Test Explorer → Run All Tests**.

Alternatively, from the solution directory:

```powershell
dotnet test
```

The integration test requires SQL Server LocalDB.

## System Design

Architecture, UML diagrams, database design and scalability considerations are documented in [docs/SystemDesign.md](docs/SystemDesign.md).

## Notes

- Run in Development mode for automatic database initialization, sample data and Swagger.
- Sample data is inserted only when the Leads table is empty.
- The application uses SQL Server LocalDB and does not require a separate SQL script.
