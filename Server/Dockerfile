# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/MyVideoResume.Server.csproj", "Server/"]
COPY ["Client/MyVideoResume.Client.csproj", "Client/"]
COPY ["MyVideoResume.AI/MyVideoResume.AI.csproj", "MyVideoResume.AI/"]
COPY ["MyVideoResume.Abstractions/MyVideoResume.Abstractions.csproj", "MyVideoResume.Abstractions/"]
COPY ["MyVideoResume.Documents/MyVideoResume.Documents.csproj", "MyVideoResume.Documents/"]
COPY ["MyVideoResume.Data/MyVideoResume.Data.csproj", "MyVideoResume.Data/"]
COPY ["MyVideoResume.Extensions/MyVideoResume.Extensions.csproj", "MyVideoResume.Extensions/"]
COPY ["MyVideoResume.Web.Common/MyVideoResume.Web.Common.csproj", "MyVideoResume.Web.Common/"]
COPY ["MyVideoResume.AI.Web/MyVideoResume.AI.Web.csproj", "MyVideoResume.AI.Web/"]
COPY ["MyVideoResume.Application/MyVideoResume.Application.csproj", "MyVideoResume.Application/"]
COPY ["MyVideoResume.ML.SentimentAnalysis/MyVideoResume.ML.SentimentAnalysis.csproj", "MyVideoResume.ML.SentimentAnalysis/"]
COPY ["MyVideoResume.Workers/MyVideoResume.Workers.csproj", "MyVideoResume.Workers/"]
RUN dotnet restore "./Server/MyVideoResume.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "./MyVideoResume.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyVideoResume.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyVideoResume.Server.dll"]