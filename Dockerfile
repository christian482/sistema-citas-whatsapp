# Usar la imagen base de .NET 8 SDK para la compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código fuente
COPY . ./

# Compilar y publicar la aplicación
RUN dotnet publish -c Release -o out

# Usar la imagen base de .NET 8 Runtime para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados desde la etapa de build
COPY --from=build /app/out .

# Exponer el puerto que usa la aplicación
EXPOSE 8080

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "citas.dll"]
