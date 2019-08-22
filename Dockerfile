FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update
RUN apt-get install nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

RUN apt-get update
RUN apt-get install nodejs

# copy csproj and restore as distinct layers
COPY src/BlackCountryBot.Tweet.Container/BlackCountryBot.Core/BlackCountryBot.Core.csproj src/BlackCountryBot.Tweet.Container/BlackCountryBot.Core/BlackCountryBot.Core.csproj
COPY src/BlackCountryBot.Web/BlackCountryBot.Web.csproj src/BlackCountryBot.Web/BlackCountryBot.Web.csproj

RUN dotnet restore "src/BlackCountryBot.Web/BlackCountryBot.Web.csproj"

# copy and publish app and libraries
COPY . . 
WORKDIR /src/src/BlackCountryBot.Web

FROM build as publish
RUN dotnet publish -c Release -o /app -r linux-arm

# Build runtime image
FROM base AS final
WORKDIR /app

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlackCountryBot.Web.dll"]
