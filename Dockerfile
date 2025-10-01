# Dockerfile que GARANTIZA la existencia de citas.dll compilando en contenedor
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar archivos de proyecto primero (para cache de Docker)
COPY ["citas.csproj", "./"]
RUN dotnet restore "citas.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar y publicar
RUN echo "=== COMPILANDO APLICACION ===" && \
    dotnet build "citas.csproj" -c Release -o /app/build && \
    echo "=== PUBLICANDO APLICACION ===" && \
    dotnet publish "citas.csproj" -c Release -o /app/publish --no-restore && \
    echo "=== ARCHIVOS GENERADOS ===" && \
    ls -la /app/publish/ && \
    echo "=== VERIFICANDO citas.dll ===" && \
    ls -la /app/publish/citas.dll

# Imagen runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar archivos compilados desde build stage
COPY --from=build /app/publish .

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Verificar y ejecutar - citas.dll DEBE estar aquí
CMD echo "=== INICIANDO APLICACION COMPILADA ===" && \
    echo "Puerto: ${PORT:-5000}" && \
    echo "=== ARCHIVOS EN CONTENEDOR RUNTIME ===" && \
    ls -la && \
    echo "=== VERIFICANDO citas.dll FINAL ===" && \
    if [ -f "citas.dll" ]; then \
        echo "✅ SUCCESS: citas.dll encontrado, ejecutando..." && \
        dotnet citas.dll --urls "http://0.0.0.0:${PORT:-5000}"; \
    else \
        echo "❌ CRITICAL ERROR: citas.dll no existe después de build" && \
        echo "Esto no debería pasar nunca con Dockerfile.build" && \
        exit 1; \
    fi
