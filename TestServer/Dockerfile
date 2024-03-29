#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["server/TestServer/nuget.config", "server/TestServer/"]
COPY ["nuget.config", "."]
COPY ["server/TestServer/TestServer.csproj", "server/TestServer/"]
COPY ["server/ServerShared/ServerShared.csproj", "server/ServerShared/"]
COPY ["shared/MasterDataServer/MasterDataServer.csproj", "shared/MasterDataServer/"]
COPY ["shared/MasterDataTypes/MasterDataTypes.csproj", "shared/MasterDataTypes/"]
COPY ["shared/Protocols/Protocols.csproj", "shared/Protocols/"]
RUN dotnet restore "server/TestServer/TestServer.csproj"
COPY . .
WORKDIR "/src/server/TestServer"
RUN dotnet build "TestServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestServer.dll"]