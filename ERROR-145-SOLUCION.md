# 🔧 Solución para Error 145 en Render.com

## ❌ Problema
```
Exited with status 145 while running your code.
```

## ✅ Soluciones Implementadas

### 🎯 **Causa del Error 145**
- Problema de configuración de puerto
- Incompatibilidades de Entity Framework
- Comando de inicio incorrecto

### 🔧 **Cambios Realizados**

#### 1. **Entity Framework Compatible**
```xml
<!-- Versiones compatibles -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.8" />
```

#### 2. **Dockerfile con Script de Debugging**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish/ ./
COPY start.sh ./
RUN chmod +x start.sh
CMD ["./start.sh"]
```

#### 3. **Script de Inicio Robusto** (`start.sh`)
- ✅ Verificación de archivos
- ✅ Configuración de puerto dinámico
- ✅ Logging detallado
- ✅ Manejo de errores

#### 4. **Puerto Dinámico Correcto**
```bash
# En start.sh
PORT=${PORT:-5000}
exec dotnet citas.dll --urls "http://0.0.0.0:$PORT"
```

### 🚀 **Para Render.com**

#### Variables de entorno necesarias:
```
ASPNETCORE_ENVIRONMENT=Production
WHATSAPP_TOKEN=tu_token_real
WHATSAPP_API_URL=https://graph.facebook.com/v19.0/TU_NUMERO/messages
```

#### Configuración Render:
- **Environment**: Docker
- **Build Command**: (dejar vacío)
- **Start Command**: (dejar vacío)

### ✅ **Verificación**

#### Logs esperados en Render:
```
=== Sistema de Citas WhatsApp - Iniciando ===
Puerto configurado: 10000
✓ citas.dll encontrado
✓ WHATSAPP_TOKEN configurado
=== Iniciando aplicación .NET ===
Aplicación iniciando en: http://0.0.0.0:10000
Now listening on: http://0.0.0.0:10000
```

#### Endpoints para probar:
- `https://tu-app.onrender.com/health` → `{"status":"healthy"}`

### 📋 **Si el error persiste**

1. **Verificar logs** en dashboard de Render
2. **Probar Dockerfile alternativo**: usar `Dockerfile.minimal` 
3. **Variables de entorno**: verificar que estén configuradas
4. **Puerto**: Render asigna automáticamente via `$PORT`

**Error 145 = problema de inicio. Esta solución añade debugging completo para identificar la causa exacta.**
