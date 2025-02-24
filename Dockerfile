# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy only necessary files first (better for caching)
COPY ["JsonWebToken/*.csproj", "./"]
RUN dotnet restore

# Copy the rest of the project files
COPY . ./
RUN dotnet publish -c Release -o /out

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
CMD ["dotnet", "JsonWebToken.dll"]
