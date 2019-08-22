FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm@6.11.1 yarn@1.17.3


FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm@6.11.1 yarn@1.17.3

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
