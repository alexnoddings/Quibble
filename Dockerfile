FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy projects
RUN cd ../
## Client
COPY ["Directory.Build.props", ""]
COPY ["Client/Client.csproj", "Client/"]
COPY ["Client.Sync/Client.Sync.csproj", "Client.Sync/"]
COPY ["Client.Sync.SignalR/Client.Sync.SignalR.csproj", "Client.Sync.SignalR/"]
COPY ["Client.Tests/Client.Tests.csproj", "Client.Tests/"]
## Server
COPY ["Server/Server.csproj", "Server/"]
## Shareds
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Shared.Sync.SignalR/Shared.Sync.SignalR.csproj", "Shared.Sync.SignalR/"]

# Restore
RUN dotnet restore "Server/Server.csproj"
COPY . .

# Build
WORKDIR "/src/Server"
RUN dotnet build "Server.csproj" -c Release -o /app/build

# Test
WORKDIR "/src/Client.Tests"
RUN dotnet restore "Client.Tests.csproj"
RUN dotnet test "Client.Tests.csproj" -c Release

# Publish
WORKDIR "/src/Server"
FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuibbleServer.dll"]
