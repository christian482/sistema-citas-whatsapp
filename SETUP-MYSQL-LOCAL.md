# 🔧 CONFIGURACIÓN MYSQL LOCAL CON XAMPP

## 📥 **PASO 1: INSTALAR XAMPP**

### **Descargar e Instalar:**
1. Ve a: https://www.apachefriends.org/download.html
2. Descarga **XAMPP para Windows**
3. Ejecuta el instalador como administrador
4. Instala con configuración por defecto

### **Iniciar Servicios:**
1. Abre **XAMPP Control Panel** 
2. Haz clic en **"Start"** para **MySQL**
3. Haz clic en **"Start"** para **Apache** (para phpMyAdmin)

## 🗄️ **PASO 2: CREAR BASE DE DATOS**

### **Acceder a phpMyAdmin:**
1. Ve a: http://localhost/phpmyadmin/
2. Usuario: `root`
3. Contraseña: (vacía - solo presiona Enter)

### **Crear Base de Datos:**
1. Haz clic en "Databases"
2. Nombre: `sistema_citas_whatsapp`
3. Collation: `utf8mb4_unicode_ci`
4. Haz clic en "Create"

### **Ejecutar Script SQL:**
1. Selecciona la base de datos creada
2. Ve a la pestaña "SQL"
3. Copia y pega el contenido de `database-setup.sql`
4. Haz clic en "Go"

## ⚙️ **PASO 3: CONFIGURAR CONNECTION STRING**

La connection string para XAMPP será:
```
mysql://root:@localhost:3306/sistema_citas_whatsapp
```

## 🚀 **PASO 4: CONFIGURAR LA APLICACIÓN**

Ya configuraremos el `appsettings.json` para usar esta conexión local.

## ✅ **VERIFICAR INSTALACIÓN**

Después de instalar XAMPP:
- MySQL debe estar corriendo en puerto 3306
- phpMyAdmin disponible en http://localhost/phpmyadmin/
- Listo para configurar la aplicación

---

**¿Prefieres instalar XAMPP o tienes otra preferencia para MySQL local?**
