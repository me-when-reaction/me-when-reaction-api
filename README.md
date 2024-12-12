```bash
dotnet ef migrations add UpdateIndexing --output-dir Infrastructure/MigrationHistory
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

Bacaan

```
https://stackoverflow.com/questions/30730937/c-sharp-fluentvalidation-for-a-hierarchy-of-classes
```
