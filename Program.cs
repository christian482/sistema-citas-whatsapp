
using citas.Data;
using citas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar variables de entorno en producción
if (builder.Environment.IsProduction())
{
    // Reemplazar placeholders con variables de entorno
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                          builder.Configuration.GetConnectionString("DefaultConnection");
    
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    builder.Configuration["WhatsApp:ApiUrl"] = Environment.GetEnvironmentVariable("WHATSAPP_API_URL") ?? 
                                               builder.Configuration["WhatsApp:ApiUrl"];
    builder.Configuration["WhatsApp:Token"] = Environment.GetEnvironmentVariable("WHATSAPP_TOKEN") ?? 
                                              builder.Configuration["WhatsApp:Token"];
}

// Configuración de DbContext
if (builder.Environment.IsProduction())
{
    // PostgreSQL para producción (Render.com)
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    // SQL Server para desarrollo
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Servicios propios
builder.Services.AddHttpClient<WhatsAppService>((sp, client) =>
{
    var config = builder.Configuration.GetSection("WhatsApp");
    var url = config["ApiUrl"] ?? "";
    var token = config["Token"] ?? "";
    client.BaseAddress = new Uri(url);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
});
builder.Services.AddScoped<WhatsAppService>(sp =>
{
    var config = builder.Configuration.GetSection("WhatsApp");
    var url = config["ApiUrl"] ?? "";
    var token = config["Token"] ?? "";
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(WhatsAppService));
    return new WhatsAppService(httpClient, url, token);
});
builder.Services.AddHostedService<ReminderBackgroundService>();

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS para producción
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
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// Configurar health checks
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
