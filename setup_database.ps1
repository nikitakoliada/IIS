$env:IsLocalEnvironment="true"
dotnet ef migrations add Initial --context ApplicationDbContext
dotnet ef database update