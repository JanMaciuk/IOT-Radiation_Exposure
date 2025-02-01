using System.Text.Json.Serialization;
using IOT.Data;
using IOT.Helpers;
using IOT.Mqtt;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Server;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(jsonConfig =>
{
    jsonConfig.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("IotDb"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); 
    }
});

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
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<MqttBroker>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors("Default");

app.Map("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

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

var  mqttServer =  scope.ServiceProvider.GetRequiredService<IOT.Mqtt.MqttBroker>();
await mqttServer.StartAsync();


app.Run();