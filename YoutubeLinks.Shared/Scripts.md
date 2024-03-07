## Docker containers links
- Api https (swagger): 
	https://localhost:5001
- Api http (swagger):  
	http://localhost:5000
- MSSQL database:
	Host: localhost 
	Port: 1433 
	Database/Schema: YoutubeLinks
	Username: sa
	Password: Password1!
	Server: youtubelinks.database
- Blazor WASM https
	https://localhost:7001
- Blazor WASM http
	http://localhost:7000

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