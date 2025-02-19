using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "My API with Swagger in .NET 9"
    });
});

// 🔹 Hangfire Konfigürasyonu (MemoryStorage)
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger'ı ana dizine yerleştirir
    });
}

// 🔹 Hangfire Dashboard Aktif Edildi
app.UseHangfireDashboard();
app.UseHangfireDashboard("/hangfire");

// 🔹 Örnek bir Hangfire Görevi (Her 10 saniyede bir çalışır)
RecurringJob.AddOrUpdate("my-recurring-job",
    () => Console.WriteLine($"[{DateTime.Now}] Hangfire Görevi Çalıştı!"),
    Cron.MinuteInterval(1) // 1 dakikada bir çalışır
);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
