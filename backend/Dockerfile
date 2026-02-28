# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore ONLY the API project
RUN dotnet restore YoutubeCompanion.API/YoutubeCompanion.API.csproj

# Publish API
RUN dotnet publish YoutubeCompanion.API/YoutubeCompanion.API.csproj \
    -c Release \
    -o out

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "YoutubeCompanion.API.dll"]