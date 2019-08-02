FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY BlackCountryBot.sln BlackCountryBot.sln

COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/EventBus.csproj
COPY BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj
COPY Services/Bot/Bot.API/Bot.API.csproj Services/Bot/Bot.API/Bot.API.csproj
COPY Services/Bot/Bot.BackgroundTasks/Bot.BackgroundTasks.csproj Services/Bot/Bot.BackgroundTasks/Bot.BackgroundTasks.csproj
COPY Services/Bot/Bot.Domain/Bot.Domain.csproj Services/Bot/Bot.Domain/Bot.Domain.csproj
COPY Services/Bot/Bot.FunctionalTests/Bot.FunctionalTests.csproj Services/Bot/Bot.FunctionalTests/Bot.FunctionalTests.csproj
COPY Services/Bot/Bot.Infrastructure/Bot.Infrastructure.csproj Services/Bot/Bot.Infrastructure/Bot.Infrastructure.csproj
COPY Web/WebSPA/WebSPA.csproj Web/WebSPA/WebSPA.csproj
COPY "Web/WebStatus/WebStatus.csproj" "Web/WebStatus/WebStatus.csproj"

COPY docker-compose.dcproj docker-compose.dcproj

RUN dotnet restore BlackCountryBot.sln

COPY . .
WORKDIR /src/src/Web/WebStatus
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]