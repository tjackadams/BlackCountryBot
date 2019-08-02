FROM resin/rpi-raspbian as qemu 
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

RUN apt-get update -yq && apt-get install -yq curl git apt-utils
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3

# copy csproj and restore as distinct layers
COPY common/WebHost.Customization/WebHost.Customization.csproj common/WebHost.Customization/WebHost.Customization.csproj
COPY src/Bot.API/Bot.API.csproj src/Bot.API/Bot.API.csproj
COPY src/Bot.Domain/Bot.Domain.csproj src/Bot.Domain/Bot.Domain.csproj
COPY src/Bot.Infrastructure/Bot.Infrastructure.csproj src/Bot.Infrastructure/Bot.Infrastructure.csproj
COPY src/Shared/Shared.csproj src/Shared/Shared.csproj

COPY src/Bot.API/ClientApp/package.json src/Bot.API/ClientApp/package.json
COPY src/Bot.API/ClientApp/yarn.lock src/Bot.API/ClientApp/yarn.lock

RUN dotnet restore "src/Bot.API/Bot.API.csproj"

WORKDIR /src/src/Bot.API/ClientApp
RUN yarn install --frozen-lockfile --no-cache --production

WORKDIR /src

# copy and publish app and libraries
COPY . . 
WORKDIR /src/src/Bot.API
RUN dotnet publish -c Release -o /app -r linux-arm

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim-arm32v7 AS runtime
WORKDIR /app

COPY --from=qemu /usr/bin/qemu-arm-static /usr/bin/qemu-arm-static 
RUN apt-get update -yq && apt-get install -yq curl git apt-utils
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3
RUN rm -f /usr/bin/qemu-arm-static

COPY --from=build /app .
ENTRYPOINT ["dotnet", "Bot.API.dll"]
