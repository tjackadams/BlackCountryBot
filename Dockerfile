FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-stretch-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3

# Install ASP.NET Core
ENV ASPNETCORE_VERSION 2.2.6

RUN curl -SL --output aspnetcore.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/$ASPNETCORE_VERSION/aspnetcore-runtime-$ASPNETCORE_VERSION-linux-arm.tar.gz \
    && aspnetcore_sha512='349fabb7bf1a2fc68a51d57cc0a12c84a333d98f53ac74338568512e1ec2f3d55fa7ee765cc690fbfa4d0d84a6e8bdc783fa42b60aaf7f65fcfaeb8e14656ef8' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf aspnetcore.tar.gz -C /usr/share/dotnet \
    && rm aspnetcore.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch-arm32v7 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential
RUN npm config set unsafe-perm true
RUN npm install -g npm@6.11.1 yarn@1.17.3

# copy csproj and restore as distinct layers
COPY src/BlackCountryBot.Tweet.Container/BlackCountryBot.Core/BlackCountryBot.Core.csproj src/BlackCountryBot.Tweet.Container/BlackCountryBot.Core/BlackCountryBot.Core.csproj
COPY src/BlackCountryBot.Web/BlackCountryBot.Web.csproj src/BlackCountryBot.Web/BlackCountryBot.Web.csproj

RUN dotnet restore "src/BlackCountryBot.Web/BlackCountryBot.Web.csproj" -r linux-arm

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
