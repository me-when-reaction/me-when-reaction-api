

```bash
dotnet ef migrations add InitialCreate --context MeWhenDBContext --output-dir Domain/Migration/PostgreSQLMigration
dotnet ef migrations remove --context MeWhenDBContext
dotnet ef database update --context MeWhenDBContext
dotnet ef database update 0 --context MeWhenDBContext
```