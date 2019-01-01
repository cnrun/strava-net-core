FROM microsoft/aspnetcore-build:2.0 
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY */*.csproj ./
RUN dotnet restore Strava.NetCore

# Copy everything else and build
COPY . ./
RUN dotnet publish Strava.NetCore/Strava.NetCore.csproj -c Release -o out

ENTRYPOINT ["dotnet", "out/aspnetapp.dll"]