using citas.Data;
using citas.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurar variables de entorno en producción
if (builder.Environment.IsProduction())
{
    var prodConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                              builder.Configuration.GetConnectionString("DefaultConnection");
    
    builder.Configuration["ConnectionStrings:DefaultConnection"] = prodConnectionString;
    builder.Configuration["WhatsApp:ApiUrl"] = Environment.GetEnvironmentVariable("WHATSAPP_API_URL") ?? 
                                               builder.Configuration["WhatsApp:ApiUrl"];
    builder.Configuration["WhatsApp:Token"] = Environment.GetEnvironmentVariable("WHATSAPP_TOKEN") ?? 
                                              builder.Configuration["WhatsApp:Token"];
}

// Configuración de DbContext - MySQL local para desarrollo y producción
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options =>
{
    Console.WriteLine($"🗄️ Configurando MySQL - Ambiente: {builder.Environment.EnvironmentName}");
    Console.WriteLine($"🔗 Conectando a: {connectionString.Replace("Pwd=", "Pwd=***")}");
    
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(true);
        options.EnableDetailedErrors(true);
        Console.WriteLine("🔍 Logging detallado habilitado");
    }
});

// Servicios propios
builder.Services.AddHttpClient<WhatsAppService>();
builder.Services.AddScoped<WhatsAppService>(sp =>
{
    var config = builder.Configuration.GetSection("WhatsApp");
    var url = config["ApiUrl"] ?? "";
    var token = config["Token"] ?? "";
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new WhatsAppService(httpClient, url, token);
});

// Controladores con configuración JSON para ciclos de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar puerto dinámico para Render.com
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var urls = $"http://localhost:{port}";
app.Urls.Clear();
app.Urls.Add(urls);

Console.WriteLine($"🚀 Aplicación iniciando en: {urls}");

// Crear base de datos si no existe y aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("📊 Verificando conexión MySQL...");
        
        if (await dbContext.Database.CanConnectAsync())
        {
            Console.WriteLine("✅ Conexión MySQL exitosa");
            await dbContext.Database.EnsureCreatedAsync();
            Console.WriteLine("🗄️ Base de datos inicializada correctamente");
            
            var businessCount = await dbContext.Businesses.CountAsync();
            Console.WriteLine($"📈 Base de datos contiene {businessCount} negocios");
        }
        else
        {
            Console.WriteLine("❌ No se pudo conectar a MySQL");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error de MySQL: {ex.Message}");
        Console.WriteLine("💡 Verifica que XAMPP esté corriendo y la BD creada");
    }
}

// Endpoint específico para verificar base de datos
app.MapGet("/health/database", async (AppDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (!canConnect)
        {
            return Results.Json(new { status = "error", message = "Cannot connect to database" }, statusCode: 503);
        }

        var stats = new
        {
            businesses = await dbContext.Businesses.CountAsync(),
            services = await dbContext.Services.CountAsync(),
            doctors = await dbContext.Doctors.CountAsync(),
            clients = await dbContext.Clients.CountAsync(),
            appointments = await dbContext.Appointments.CountAsync(),
            activeAppointments = await dbContext.Appointments.CountAsync(a => !a.IsCancelled)
        };

        // También obtener algunos datos de ejemplo
        var businessesList = await dbContext.Businesses.Take(3).ToListAsync();
        var doctorsList = await dbContext.Doctors.Take(3).ToListAsync();

        return Results.Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            database = "MySQL Local (XAMPP)",
            connectionString = "***HIDDEN***",
            statistics = stats,
            sampleData = new
            {
                businesses = businessesList,
                doctors = doctorsList
            }
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new 
        { 
            status = "error", 
            message = ex.Message,
            timestamp = DateTime.UtcNow
        }, statusCode: 503);
    }
});

// Health check básico
app.MapGet("/health", async (AppDbContext dbContext) => 
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        var businessCount = canConnect ? await dbContext.Businesses.CountAsync() : 0;
        
        return Results.Ok(new { 
            status = canConnect ? "healthy" : "database_error",
            timestamp = DateTime.UtcNow,
            port = port,
            database = "MySQL Local (XAMPP)",
            connection = canConnect ? "connected" : "disconnected",
            businesses = businessCount
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { 
            status = "error",
            message = ex.Message,
            timestamp = DateTime.UtcNow
        }, statusCode: 503);
    }
});

// Página de inicio
app.MapGet("/", () => Results.Text($@"
🎉 SISTEMA DE CITAS CON WHATSAPP - LOCAL XAMPP ✅

📊 Estado: Activo
🗄️ Base de datos: MySQL Local (XAMPP)
🕒 Hora: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
🌐 Puerto: {port}
🔧 Ambiente: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}

📋 ENDPOINTS DISPONIBLES:
• GET  /health - Estado del sistema
• GET  /api/businesses - Listar negocios  
• POST /api/businesses - Crear negocio
• GET  /api/doctors - Listar doctores
• POST /api/doctors - Crear doctor
• GET  /api/clients - Listar clientes
• POST /api/clients - Crear cliente
• GET  /api/services - Listar servicios
• POST /api/services - Crear servicio
• GET  /api/appointments - Listar citas
• POST /api/appointments - Crear cita

📱 WEBHOOK WHATSAPP:
• POST /webhook/whatsapp - Recibir mensajes
• GET  /webhook/whatsapp - Verificación webhook

🚀 Ejecutándose localmente con XAMPP MySQL

📖 DOCUMENTACION API: /swagger
", "text/plain"));

// Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Citas WhatsApp API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("✅ Aplicación lista - usando MySQL local");
app.Run();
