dotnet --version
dotnet restore MGInput.csproj
dotnet build MGInput.csproj
dotnet pack --include-source MGInput.csproj