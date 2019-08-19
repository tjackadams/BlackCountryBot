FROM openfaas/classic-watchdog:0.15.4 as watchdog

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch-arm32v7  AS build

# Supress collection of data.
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Optimize for Docker builder caching by adding projects first.
RUN mkdir -p /home/app/src/BlackCountryBot.Core
WORKDIR /home/app/src/BlackCountryBot.Core
COPY ./BlackCountryBot.Core/BlackCountryBot.Core.csproj .

RUN mkdir -p /home/app/src/BlackCountryBot.Tweet
WORKDIR /home/app/src/BlackCountryBot.Tweet
COPY ./BlackCountryBot.Tweet/BlackCountryBot.Tweet.csproj .

WORKDIR /home/app/src/
COPY ./BlackCountryBot.Tweet.Container.csproj  .
RUN dotnet restore ./BlackCountryBot.Tweet.Container.csproj

COPY . .

RUN dotnet publish -c Release -o published

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-stretch-slim-arm32v7

# Supress collection of data.
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

COPY --from=watchdog /fwatchdog /usr/bin/fwatchdog
RUN chmod +x /usr/bin/fwatchdog

# Create a non-root user
RUN addgroup --system app \
    && adduser --system --ingroup app app

WORKDIR /home/app/
COPY --from=build /home/app/src/published .
RUN chown app:app -R /home/app

USER app

ENV fprocess="dotnet ./root.dll"
EXPOSE 8080

HEALTHCHECK --interval=3s CMD [ -e /tmp/.lock ] || exit 1

CMD ["fwatchdog"]