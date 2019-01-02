# https://docs.docker.com/engine/examples/dotnetcore/#create-a-dockerfile-for-an-aspnet-core-application
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Strava.NetCore/*.csproj ./Strava.NetCore/
COPY stravadotnet/StravaDotNet/*.csproj ./stravadotnet/StravaDotNet/
COPY AspNet.Security.OAuth.Providers/src/AspNet.Security.OAuth.Strava/*.csproj ./AspNet.Security.OAuth.Providers/src/AspNet.Security.OAuth.Strava/
RUN dotnet restore Strava.NetCore/Strava.NetCore.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish Strava.NetCore/Strava.NetCore.csproj -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]