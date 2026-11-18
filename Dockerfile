# Luqma API — Docker image for Render (Web Service, Docker runtime)
# .NET 8, multi-stage build. Listens on Render's injected $PORT (defaults to 10000 locally).

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Trust Render's reverse proxy (X-Forwarded-Proto/For) so request
    # URLs/schemes and UseHttpsRedirection behave correctly behind TLS termination.
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution + project files first for better layer caching of `dotnet restore`
COPY ["Luqma.sln", "./"]
COPY ["Luqma.api/Luqma.api.csproj", "Luqma.api/"]
COPY ["Luqma.Application/Luqma.Application.csproj", "Luqma.Application/"]
COPY ["Luqma.Domain/Luqma.Domain.csproj", "Luqma.Domain/"]
COPY ["Luqma.Infrastructure/Luqma.Infrastructure.csproj", "Luqma.Infrastructure/"]
RUN dotnet restore "Luqma.sln"

# Copy the rest of the source and publish
COPY . .
WORKDIR "/src/Luqma.api"
RUN dotnet publish "Luqma.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
# Render injects $PORT at runtime — bind to it. Shell form expands $PORT inside the container.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet Luqma.api.dll"]
