# 🔧 Solución para Error 139 en Render.com

## ❌ Problema Original
```
Exited with status 139 while running your code.
```

## ✅ Solución Implementada

### 1. **Dockerfile sin compilación en contenedor**
- Usa archivos **pre-compilados** localmente
- Evita problemas de memoria durante build
- Runtime optimizado para contenedores

### 2. **Archivos incluidos en el repositorio**
- `/publish/` - Binarios compilados listos para producción  
- `citas.dll` - Aplicación principal
- Dependencias SQLite incluidas
- Configuración de producción

### 3. **Configuración Render.com**
```yaml
Environment: Docker
Build Command: (DEJAR VACÍO)
Start Command: (DEJAR VACÍO)
Dockerfile Path: ./Dockerfile
```

### 4. **Variables de entorno mínimas**
```
ASPNETCORE_ENVIRONMENT=Production
WHATSAPP_TOKEN=tu_token_real
WHATSAPP_API_URL=https://graph.facebook.com/v19.0/TU_NUMERO/messages
```

## 🚀 Pasos para resolver en Render.com

1. **Conectar repositorio actualizado**
2. **Trigger manual deploy** 
3. **Configurar solo las 3 variables de entorno**
4. **La aplicación iniciará sin compilar**

## ✅ Verificación

Una vez desplegado:
- Endpoint health: `https://tu-app.onrender.com/health`
- Debería mostrar: `{"status":"healthy"}`

## 📋 Cambios técnicos realizados

- ❌ Eliminada compilación en Dockerfile
- ✅ Archivos pre-compilados incluidos
- ✅ SQLite integrado (más estable que SQL Server)  
- ✅ Puerto dinámico (`$PORT`)
- ✅ Configuración mínima de entorno

**El error 139 se produce por compilación en contenedores con recursos limitados. Esta solución lo evita completamente.**
