FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY BlackCountryBot.sln BlackCountryBot.sln

COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/
COPY BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj BuildingBlocks/WebHost.Customization/
COPY Services/Bot/Bot.API/Bot.API.csproj Services/Bot/Bot.API/
COPY Services/Bot/Bot.BackgroundTasks/Bot.BackgroundTasks.csproj Services/Bot/Bot.BackgroundTasks/
COPY Services/Bot/Bot.Domain/Bot.Domain.csproj Services/Bot/Bot.Domain/
COPY Services/Bot/Bot.FunctionalTests/Bot.FunctionalTests.csproj Services/Bot/Bot.FunctionalTests/
COPY Services/Bot/Bot.Infrastructure/Bot.Infrastructure.csproj Services/Bot/Bot.Infrastructure/
COPY Web/WebSPA/WebSPA.csproj Web/WebSPA/

COPY docker-compose.dcproj docker-compose.dcproj

RUN dotnet restore BlackCountryBot.sln

COPY . .
WORKDIR /src/src/Services/Bot/Bot.BackgroundTasks
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bot.BackgroundTasks.dll"]