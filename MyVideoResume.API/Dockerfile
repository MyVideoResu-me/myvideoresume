# Use the official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyVideoResume.API/MyVideoResume.API.csproj", "MyVideoResume.API/"]
RUN dotnet restore "MyVideoResume.API/MyVideoResume.API.csproj"
COPY . .
WORKDIR "/src/MyVideoResume.API"
RUN dotnet build "MyVideoResume.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyVideoResume.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyVideoResume.API.dll"]

# Add hot reload
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS hotreload
WORKDIR /src
COPY . .
WORKDIR "/src/MyVideoResume.API"
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:80"]