# FaceLook - а chat messaging app

## Preresquitives:
- Microsoft SQL Server/ Microsoft SQL Server Express
- Working SMTP server (or using third party provided SMTP server).


## Running
1. Fill appsettings.json with all desired values

2.Execute the following commands in Powershell in order:

2.1.` dotnet ef database update --project Facelook/Facelook.csproj` or `Update-Database` in Package Management Console.
  
2.2 `dotnet restore`
  
2.3. `dotnet run --project Facelook/Facelook.csproj`
