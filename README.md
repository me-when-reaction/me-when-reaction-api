```bash
dotnet ef migrations add InitialCreate --context MeWhenDBContext --output-dir Infrastructure/Migration
dotnet ef migrations remove --context MeWhenDBContext
dotnet ef database update --context MeWhenDBContext
dotnet ef database update 0 --context MeWhenDBContext
```