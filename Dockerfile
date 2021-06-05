FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# App build/publish
RUN cd ../
COPY ["Directory.Build.props", ""]
COPY ["Server/Server.csproj", "Server/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Client/Client.csproj", "Client/"]
COPY ["Client.Synchronisation/Client.Sync.csproj", "Client.Synchronisation/"]
RUN dotnet restore "Server/Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuibbleServer.dll"]
