# Use the official .NET 6 container
FROM mcr.microsoft.com/dotnet/sdk:6.0

# Copy files from the current folder on your machine to 
# the /src folder in the container
COPY ./ /src

# Set working directory in the container
WORKDIR /src

# Expose HTTP port from launchSettings.json
EXPOSE 5101

# Run the application when the container is started
ENTRYPOINT ["dotnet", "run", "--launch-profile", "docker"]
