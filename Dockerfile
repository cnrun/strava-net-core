# https://docs.docker.com/engine/examples/dotnetcore/#create-a-dockerfile-for-an-aspnet-core-application
# Migrating from aspnetcore docker repos to dotnet
# https://github.com/aspnet/Announcements/issues/298
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
#setup node
ENV NODE_VERSION 10.15.0
# Linux
ENV NODE_DOWNLOAD_SHA f0b4ff9a74cbc0106bbf3ee7715f970101ac5b1bbe814404d7a0673d1da9f674
# Darwin
# ENV NODE_DOWNLOAD_SHA 353402461c898c569658d0a963790476f4d9828cc6c9286d81617ee8afcba4e8
# ENV NODE_VERSION 8.9.4
# Linux
# ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
# Darwin
#ENV NODE_DOWNLOAD_SHA ca50f7d2035eb805306e303b644bb1cde170ce2615e0a2c6e95fb80881c48c24
# linux or darwin
ENV NODE_OS linux

RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-${NODE_OS}-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
# RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano apt-utils
# RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential

# Copy csproj and restore as distinct layers
COPY Strava.NetCore/*.csproj ./Strava.NetCore/
COPY stravadotnet/StravaDotNet/*.csproj ./stravadotnet/StravaDotNet/
COPY AspNet.Security.OAuth.Providers/src/AspNet.Security.OAuth.Strava/*.csproj ./AspNet.Security.OAuth.Providers/src/AspNet.Security.OAuth.Strava/
RUN dotnet restore Strava.NetCore/Strava.NetCore.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish Strava.NetCore/Strava.NetCore.csproj -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
#setup node, this is only needed if you use Node both at runtime and build time. Some people may only need the build part.
ENV NODE_VERSION 10.15.0
# Linux
ENV NODE_DOWNLOAD_SHA f0b4ff9a74cbc0106bbf3ee7715f970101ac5b1bbe814404d7a0673d1da9f674
# Darwin
# ENV NODE_DOWNLOAD_SHA 353402461c898c569658d0a963790476f4d9828cc6c9286d81617ee8afcba4e8
ENV NODE_OS linux

RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-${NODE_OS}-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
