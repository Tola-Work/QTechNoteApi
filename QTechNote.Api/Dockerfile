# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files first
COPY ["../QTechNote.Api/QTechNote.Api.csproj", "QTechNote.Api/"]
COPY ["../QTechNote.Data/QTechNote.Data.csproj", "QTechNote.Data/"]
COPY ["../QTechNote.Models/QTechNote.Models.csproj", "QTechNote.Models/"]

# Restore dependencies
RUN dotnet restore "QTechNote.Api/QTechNote.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/QTechNote.Api"
RUN dotnet build "QTechNote.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "QTechNote.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QTechNote.Api.dll"]