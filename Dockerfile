FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
WORKDIR /src

# set up node
ENV NODE_VERSION 11.11.0
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
ENV NODE_DOWNLOAD_SHA f749e64a56dc71938fa5d2774b4e53068d19ad9f48b4a62257633b25459bffa6

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# set up yarn
ENV YARN_VERSION 1.13.0

RUN set -ex \
  && wget -qO- https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --import \
  && curl -fSLO --compressed "https://yarnpkg.com/downloads/$YARN_VERSION/yarn-v$YARN_VERSION.tar.gz" \
  && curl -fSLO --compressed "https://yarnpkg.com/downloads/$YARN_VERSION/yarn-v$YARN_VERSION.tar.gz.asc" \
  && gpg --batch --verify yarn-v$YARN_VERSION.tar.gz.asc yarn-v$YARN_VERSION.tar.gz \
  && mkdir -p /opt/yarn \
  && tar -xzf yarn-v$YARN_VERSION.tar.gz -C /opt/yarn --strip-components=1 \
  && ln -s /opt/yarn/bin/yarn /usr/local/bin/yarn \
  && ln -s /opt/yarn/bin/yarn /usr/local/bin/yarnpkg \
  && rm yarn-v$YARN_VERSION.tar.gz.asc yarn-v$YARN_VERSION.tar.gz

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

# set up node
ENV NODE_VERSION 11.11.0
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
ENV NODE_DOWNLOAD_SHA f749e64a56dc71938fa5d2774b4e53068d19ad9f48b4a62257633b25459bffa6

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# set up yarn
ENV YARN_VERSION 1.13.0

RUN set -ex \
  && wget -qO- https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --import \
  && curl -fSLO --compressed "https://yarnpkg.com/downloads/$YARN_VERSION/yarn-v$YARN_VERSION.tar.gz" \
  && curl -fSLO --compressed "https://yarnpkg.com/downloads/$YARN_VERSION/yarn-v$YARN_VERSION.tar.gz.asc" \
  && gpg --batch --verify yarn-v$YARN_VERSION.tar.gz.asc yarn-v$YARN_VERSION.tar.gz \
  && mkdir -p /opt/yarn \
  && tar -xzf yarn-v$YARN_VERSION.tar.gz -C /opt/yarn --strip-components=1 \
  && ln -s /opt/yarn/bin/yarn /usr/local/bin/yarn \
  && ln -s /opt/yarn/bin/yarn /usr/local/bin/yarnpkg \
  && rm yarn-v$YARN_VERSION.tar.gz.asc yarn-v$YARN_VERSION.tar.gz

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlackCountryBot.Web.dll"]
