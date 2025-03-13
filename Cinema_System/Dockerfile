# Use the official .NET 8 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["Cinema_System.csproj", "./"]
RUN dotnet restore

# Copy the entire project and build it
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Use the ASP.NET Core runtime to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Copy the built application from the previous stage
COPY --from=build /app/publish .

# Expose port 8080 for Fly.io, Render, or any cloud provider
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "CinemaSystem.dll"]
