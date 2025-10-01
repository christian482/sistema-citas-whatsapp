# 🗄️ CONFIGURACIÓN MYSQL CON FREEMYSQLHOSTING

## 📋 PASOS PARA CONFIGURAR LA BASE DE DATOS

### 1. **Crear Cuenta en FreeMySQLHosting**
1. Ve a: https://www.freemysqlhosting.net/
2. Haz clic en "Register" (Registrarse)
3. Completa el formulario:
   - **Your Name**: Tu nombre
   - **Email Address**: Tu email
   - **Username**: Elige un username único
   - **Password**: Contraseña segura
   - **Confirm Password**: Confirma la contraseña

### 2. **Crear Nueva Base de Datos**
1. Después del registro, ve a "Control Panel"
2. Haz clic en "Create Database"
3. Completa la información:
   - **Database Name**: `sistema_citas_whatsapp`
   - **Description**: Sistema de citas con WhatsApp
4. Haz clic en "Create Database"

### 3. **Obtener Datos de Conexión**
Después de crear la base de datos, obtendrás:
```
Database Name: sql12xxxxxx (será un nombre generado)
Database User: sql12xxxxxx (mismo que el nombre)
Password: [tu contraseña generada]
Server: sql12.freemysqlhosting.net
Port: 3306
```

### 4. **Construir Connection String**
Con los datos anteriores, tu connection string será:
```
mysql://sql12xxxxxx:PASSWORD@sql12.freemysqlhosting.net:3306/sql12xxxxxx
```

**Ejemplo:**
```
mysql://sql12345678:mypassword123@sql12.freemysqlhosting.net:3306/sql12345678
```

### 5. **Ejecutar Script de Base de Datos**
1. En el Control Panel de FreeMySQLHosting
2. Haz clic en "phpMyAdmin" o "Manage Database"
3. Ve a la pestaña "SQL"
4. Copia y pega el contenido del archivo `database-minimal.sql`
5. Haz clic en "Go" o "Execute"

### 6. **Configurar Variable en Render.com**
1. Ve a tu dashboard de Render.com
2. Selecciona tu servicio "sistema-citas-whatsapp"
3. Ve a la pestaña **"Environment"**
4. Haz clic en "Add Environment Variable"
5. Agrega:
   - **Key**: `DATABASE_URL`
   - **Value**: Tu connection string de MySQL (paso 4)

### 7. **Verificar Conexión**
Después de configurar la variable:
1. Render.com hará un redeploy automático
2. Ve a: https://sistema-citas-whatsapp.onrender.com/health
3. Deberías ver `"database": "MySQL"` en la respuesta

## 🔧 ARCHIVOS DISPONIBLES

### `database-setup.sql` (Completo)
- Script completo con todas las funcionalidades
- Incluye datos de ejemplo, vistas, procedimientos
- **Usar si tienes MySQL completo o local**

### `database-minimal.sql` (Recomendado para FreeMySQLHosting)
- Script básico solo con tablas esenciales
- Optimizado para limitaciones de hosting gratuito
- **Usar para FreeMySQLHosting**

## ⚠️ LIMITACIONES DE FREEMYSQLHOSTING

- **Límite de queries por hora**: 2000
- **Tamaño máximo de BD**: 5MB
- **Conexiones simultáneas**: 2
- **Sin soporte para procedimientos complejos**

## 🚀 DEPLOYMENT

Una vez configurada la variable `DATABASE_URL`:

1. Tu aplicación se reconectará automáticamente
2. Las tablas se crearán automáticamente con `EnsureCreated()`
3. El sistema estará listo para usar

## 📱 PRUEBAS

Para probar que funciona:
1. Ve a: https://sistema-citas-whatsapp.onrender.com/swagger
2. Prueba crear un negocio, doctor, cliente y cita
3. Verifica en phpMyAdmin que los datos se guardan

## 🔄 DATOS DE EJEMPLO

El script `database-minimal.sql` incluye:
- 1 Negocio: "Clínica Demo"
- 1 Servicio: "Consulta General"
- 1 Doctor: "Dr. Demo"
- 1 Cliente: "Cliente Prueba"

¡Listos para empezar a usar el sistema! 🎉
