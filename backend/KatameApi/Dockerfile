FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY KatameApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway/Render/Fly inyectan PORT en tiempo de ejecución (no en build), así que el
# bind de Kestrel se arma en el entrypoint en vez de con un ENV fijo.
ENV ASPNETCORE_ENVIRONMENT=Production
# Fijamos la zona horaria explícitamente en vez de depender de que la imagen
# base venga en UTC "por casualidad" -- todas las fechas del backend se guardan
# y comparan asumiendo UTC (ver Extensions/DateTimeExtensions.cs).
ENV TZ=UTC
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet KatameApi.dll"]
