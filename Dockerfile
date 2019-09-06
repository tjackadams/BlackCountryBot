FROM resin/rpi-raspbian as qemu 
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

RUN apt-get update -yq && apt-get install -yq curl git apt-utils
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3

# copy csproj and restore as distinct layers
COPY src/BlackCountryBot.Tweet/BlackCountryBot.Core/BlackCountryBot.Core.csproj src/BlackCountryBot.Tweet/BlackCountryBot.Core/BlackCountryBot.Core.csproj
COPY src/BlackCountryBot.Web/BlackCountryBot.Web.csproj src/BlackCountryBot.Web/BlackCountryBot.Web.csproj
COPY src/BlackCountryBot.Web/ClientApp/package.json src/BlackCountryBot.Web/ClientApp/package.json
COPY src/BlackCountryBot.Web/ClientApp/yarn.lock src/BlackCountryBot.Web/ClientApp/yarn.lock

RUN dotnet restore "src/BlackCountryBot.Web/BlackCountryBot.Web.csproj"

WORKDIR /src/src/BlackCountryBot.Web/ClientApp
RUN yarn install --frozen-lockfile --no-cache --production

WORKDIR /src

# copy and publish app and libraries
COPY . . 
WORKDIR /src/src/BlackCountryBot.Web
RUN dotnet publish -c Release -o /app -r linux-arm

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim-arm32v7 AS runtime
WORKDIR /app

COPY --from=qemu /usr/bin/qemu-arm-static /usr/bin/qemu-arm-static 
RUN apt-get update -yq && apt-get install -yq curl git apt-utils
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3
RUN rm -f /usr/bin/qemu-arm-static

COPY --from=build /app .
ENTRYPOINT ["dotnet", "BlackCountryBot.Web.dll"]
