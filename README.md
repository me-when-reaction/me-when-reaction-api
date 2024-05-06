```bash
dotnet ef migrations add InitialCreate --output-dir Infrastructure/MigrationHistory
dotnet ef migrations remove
dotnet ef database update
dotnet ef database update 0
```