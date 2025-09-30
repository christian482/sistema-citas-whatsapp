
using citas.Data;
using citas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
