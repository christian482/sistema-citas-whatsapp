using citas.Data;
using citas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar variables de entorno en producción
if (builder.Environment.IsProduction())
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Data Source=app.db";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    builder.Configuration["WhatsApp:ApiUrl"] = Environment.GetEnvironmentVariable("WHATSAPP_API_URL") ?? 
                                               builder.Configuration["WhatsApp:ApiUrl"];
    builder.Configuration["WhatsApp:Token"] = Environment.GetEnvironmentVariable("WHATSAPP_TOKEN") ?? 
                                              builder.Configuration["WhatsApp:Token"];
}

// Configuración de DbContext - SQLite para simplicidad en producción
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseSqlite("Data Source=app.db");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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

// Controladores
builder.Services.AddControllers();
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
var urls = $"http://0.0.0.0:{port}";
app.Urls.Clear();
app.Urls.Add(urls);

Console.WriteLine($"Aplicación iniciando en: {urls}");

// Crear base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Base de datos inicializada correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar base de datos: {ex.Message}");
    }
}

// Configurar health checks
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    port = port,
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Aplicación iniciada correctamente");
app.Run();
