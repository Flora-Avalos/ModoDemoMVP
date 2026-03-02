
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Data;
using ModoDemoMVP.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => 
     options.UseSqlServer(
         builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddHttpClient<ModoService>();
builder.Services.AddHttpClient<ModoService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(config["Modo:BaseUrl"]!);

    client.DefaultRequestHeaders.Add(
        "User-Agent",
        config["Modo:UserAgent"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
