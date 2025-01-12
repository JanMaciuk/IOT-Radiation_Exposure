using IOT.Data;
using IOT.Helpers;
using IOT.Mqtt;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddControllers();

builder.Services.AddNpgsql<AppDbContext>(builder.Configuration.GetConnectionString("IotDb"), null,
    dbBuilder =>
    {
        dbBuilder.UseSnakeCaseNamingConvention();
        if (builder.Environment.IsDevelopment())
        {
            dbBuilder.EnableSensitiveDataLogging();
        }
    }
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSerilog(config =>
{
    config
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
        .Enrich.FromLogContext();
});

builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<MqttClient>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors("Default");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
seeder.Seed();
scope.ServiceProvider.GetRequiredService<MqttClient>();

app.Run();