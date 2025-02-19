using Hangfire;
using Hangfire.MemoryStorage;
using HangfireWithNet9.Services;
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

#region Hangfire Konfigürasyonu (MemoryStorage)
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient(); // HTTP istekleri için HttpClient ekle
builder.Services.AddScoped<ExchangeRateService>(); // **ExchangeRateService** servisini DI konteynerine ekliyoruz.
#endregion



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
else
{
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");
#endregion

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Hangfire ile periyodik olarak servis çağır
RecurringJob.AddOrUpdate<ExchangeRateService>("fetch-exchange-rates",
                                 service => service.FetchAndUpdateRates(), Cron.MinuteInterval(2));

app.Run();
