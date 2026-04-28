# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["LMS.API/LMS.API.csproj", "LMS.API/"]
RUN dotnet restore "LMS.API/LMS.API.csproj"

COPY . .
WORKDIR "/src/LMS.API"
RUN dotnet publish "LMS.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "LMS.API.dll"]