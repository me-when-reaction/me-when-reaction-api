```bash
dotnet ef migrations add Update2 --output-dir Infrastructure/MigrationHistory
dotnet ef migrations remove
dotnet ef database update
dotnet ef database update 0
```

```cs
migrationBuilder.Sql("SET TimeZone='UTC';");
```

```bash
dotnet publish -c Release /p:EnvironmentName=Production
```
