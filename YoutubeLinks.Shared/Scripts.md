## Entity Framework Core Migrations
Remember to change ***MIGRATION_NAME*** 
``` 
cd C:\Users\bartl\source\repos\YoutubeLinks\YoutubeLinks.Api
dotnet ef migrations add MIGRATION_NAME -o ./Data/Migrations --startup-project ../YoutubeLinks.Api
dotnet ef database update
```

## Update docker-compose.yaml
``` 
cd C:\Users\bartl\source\repos\YoutubeLinks
docker compose up --build -d
docker ps
``` 

## Error with Api on Docker
If there is a problem with docker and access to the database change "Server=localhost" to "Server=youtubelinks.database" in database connection string

## Playwright installation / codegen
``` 
dotnet tool install --global PowerShell
dotnet run
pwsh .\bin\Debug\net8.0\playwright.ps1 install
pwsh .\bin\Debug\net8.0\playwright.ps1 codegen http://localhost:7000/
dotnet test
``` 