FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "BlackCountryBot.sln" "BlackCountryBot.sln"

COPY "BlackCountryBot.Web/BlackCountryBot.Web.csproj" "BlackCountryBot.Web/BlackCountryBot.Web.csproj"
COPY "BlackCountryBot.Infrastructure/BlackCountryBot.Infrastructure.csproj" "BlackCountryBot.Infrastructure/BlackCountryBot.Infrastructure.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "BlackCountryBot.sln"

COPY . .
WORKDIR /src/BlackCountryBot.Web
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlackCountryBot.Web.dll"]
