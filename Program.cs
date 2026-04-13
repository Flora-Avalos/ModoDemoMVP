using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Data;
using ModoDemoMVP.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Puerto
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Servicios
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// PostgreSQL (Npgsql)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient para Modo
builder.Services.AddHttpClient<ModoService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Modo:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("User-Agent", config["Modo:UserAgent"]!);
});

builder.Services.AddHttpClient<ModoKeysService>((sp, client) =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "ModoDemoMVP");
});

var app = builder.Build();

//aplicar migraciones automatica
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

//cargar jwks de modo al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var modoKeysService = scope.ServiceProvider.GetRequiredService<ModoKeysService>();
    await modoKeysService.LoadKeysAsync();                                      
}

// Middleware para manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Endpoint /error que muestra detalles de la excepción (útil temporalmente)
app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Problem(title: "Error interno", detail: exception?.ToString());
});

// Middlewares
// Desactivar HTTPS temporalmente si Render no tiene certificado
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Endpoints
app.MapRazorPages();
app.MapControllers();

// Endpoint de prueba rápido
app.MapGet("/api/test", () => "ok");
app.Run();