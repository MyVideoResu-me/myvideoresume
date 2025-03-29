# Use the official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Server/MyVideoResume.Server.csproj", "Server/"]
RUN dotnet restore "Server/MyVideoResume.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "MyVideoResume.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyVideoResume.Server.csproj" -c Release -o /app/publish

FROM nginx:alpine
COPY --from=publish /app/publish /usr/share/nginx/html

# Add hot reload
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS hotreload
WORKDIR /src
COPY . .
WORKDIR "/src/Server"
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:80"]