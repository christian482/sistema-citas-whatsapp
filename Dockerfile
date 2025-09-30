# Dockerfile que evita compilación en contenedor (evita error 139)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Configurar directorio de trabajo
WORKDIR /app

# Copiar archivos ya compilados
COPY publish/ ./

# Configurar variables de entorno optimizadas
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_EnableDiagnostics=0

# No exponer puerto fijo, usar variable PORT de Render.com
EXPOSE $PORT

# Ejecutar aplicación
ENTRYPOINT ["dotnet", "citas.dll"]
