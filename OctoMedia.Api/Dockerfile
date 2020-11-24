FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS publish
ARG RELEASE_VERSION
WORKDIR /src
COPY ["OctoMedia.Api/OctoMedia.Api.csproj", "OctoMedia.Api/"]
RUN dotnet restore "OctoMedia.Api/OctoMedia.Api.csproj"
COPY . .
WORKDIR "/src/OctoMedia.Api"
RUN dotnet publish "OctoMedia.Api.csproj" -c Release -p:VersionPrefix=$RELEASE_VERSION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OctoMedia.Api.dll"]