FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN  apt-get install -y nodejs
RUN curl -sL https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list
RUN apt-get update && apt-get install yarn
WORKDIR /sln

COPY ./*.sln  ./

# Copy csproj and restore as distinct layers
COPY ./src/BlackCountryBot.Core/BlackCountryBot.Core.csproj ./src/BlackCountryBot.Core/BlackCountryBot.Core.csproj
COPY ./src/BlackCountryBot.Web/BlackCountryBot.Web.csproj ./src/BlackCountryBot.Web/BlackCountryBot.Web.csproj
COPY ./test/BlackCountryBot.IntegrationTests/BlackCountryBot.IntegrationTests.csproj ./test/BlackCountryBot.IntegrationTests/BlackCountryBot.IntegrationTests.csproj
RUN dotnet restore

COPY ./test ./test
COPY ./src ./src

RUN dotnet build -c Release --no-restore

RUN dotnet publish "./src/BlackCountryBot.Web/BlackCountryBot.Web.csproj" -c Release -o "../../dist" --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:5000"
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 5000/tcp
ENTRYPOINT [ "dotnet", "BlackCountryBot.Web.dll" ]
COPY --from=build-env /sln/dist .