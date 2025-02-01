using System.Text;
using System.Text.Json;
using IOT.Data;
using IOT.Models;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Server;
using Serilog;

namespace IOT.Mqtt
{
    public class MqttBroker
    {
        private readonly AppDbContext _context;
        private readonly MqttServerOptions _options;
        private MqttServer _mqttServer;
        private IMqttClient _mqttClient;

        public MqttBroker(AppDbContext context)
        {
            _context = context;

            _options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Any)
                .Build();

            var factory = new MqttServerFactory();
            var factorycli = new MqttClientFactory();

            _mqttServer = factory.CreateMqttServer(_options);

            _mqttClient = factorycli.CreateMqttClient();

            _mqttServer.InterceptingPublishAsync += HandleIncomingMessage;
            _mqttServer.ValidatingConnectionAsync += Server_ValidatingConnectionAsync;
            _mqttServer.ClientConnectedAsync += HandleClientConnected;
            _mqttServer.ClientDisconnectedAsync += HandleClientDisconnected;
        }

        Task Server_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            if (!string.IsNullOrWhiteSpace(arg.UserName) && !string.IsNullOrWhiteSpace(arg.Password))
            {
                Console.WriteLine(arg.UserName);
                Console.WriteLine(arg.Password);
                if (arg.UserName != "admin" || arg.Password != "admin" )
                {
                    arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                    return Task.CompletedTask;
                }
                arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                return Task.CompletedTask;
            }
            arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadAuthenticationMethod;
            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            await _mqttServer.StartAsync();
            Console.WriteLine("MQTT Server started on port 1883.");

            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) 
                .WithClientId("InternalPublisher")
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(clientOptions);
        }

        public async Task StopAsync()
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }

            await _mqttServer.StopAsync();
            Console.WriteLine("MQTT Server stopped.");
        }

        private async Task HandleIncomingMessage(InterceptingPublishEventArgs args)
        {
            try
            {

                if (args.ClientId == "Server")
                {
                    return;
                }
                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                Console.WriteLine($"Received message on topic '{args.ApplicationMessage.Topic}': {payload}");

                var log = JsonSerializer.Deserialize<EntranceLog>(payload);
                if (log == null || string.IsNullOrEmpty(log.CardId))
                {
                    Console.WriteLine("Invalid or null message received.");
                    return;
                }
                Console.WriteLine(log.CardId);
                
                var employee = _context.Employees.FirstOrDefault(e => e.Card == log.CardId);
                var zone = _context.Zones.FirstOrDefault(z => z.Id == log.ZoneId);

                await ProcessMessage(log);

                var response = new EntranceResponse
                {
                    CardId = log.CardId,
                    ZoneId = log.ZoneId,
                    AccessGranted = true
                };

                if (employee == null || zone == null)
                {
                    response.AccessGranted = false;
                }
                if (IsEmployeeRadiationDoseExceeded(employee.Id))
                {
                    response.AccessGranted = false;
                }


                var responsePayload = JsonSerializer.Serialize(response);
                var responseMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"iot/entrance_response/{log.RaspberryId}")
                    .WithPayload(responsePayload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                
                await _mqttServer.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(responseMessage)
                    {
                        SenderClientId = "Server"
                    });

                Console.WriteLine($"Response sent to topic 'iot/entrance_response/{log.RaspberryId}': {responsePayload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private Task HandleClientConnected(ClientConnectedEventArgs args)
        {
            Console.WriteLine($"Client connected: {args.ClientId}");
            return Task.CompletedTask;
        }

        private Task HandleClientDisconnected(ClientDisconnectedEventArgs args)
        {
            Console.WriteLine($"Client disconnected: {args.ClientId}");
            return Task.CompletedTask;
        }

        private async Task ProcessMessage(EntranceLog log)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Card == log.CardId);
            var zone = _context.Zones.FirstOrDefault(z => z.Id == log.ZoneId);

            if (employee == null)
            {
                Console.WriteLine($"Employee not found for CardId: {log.CardId}");
                return;
            }

            if (zone == null)
            {
                Console.WriteLine($"Zone not found for ZoneId: {log.ZoneId}");
                return;
            }
            var existingLog = _context.EmployeeEntrance
                .FirstOrDefault(e => e.EmployeeId == employee.Id && e.ExitTime == null);

            if (existingLog != null)
            {
                existingLog.ExitTime = log.Timestamp;
                _context.EmployeeEntrance.Update(existingLog);
            }
            else
            {
                if(IsEmployeeRadiationDoseExceeded(employee.Id))
                {
                    return;
                }
                var newLog = new EmployeeEntrance
                {
                    EmployeeId = employee.Id,
                    ZoneId = log.ZoneId,
                    EntranceTime = log.Timestamp,
                    ExitTime = null
                };

                _context.EmployeeEntrance.Add(newLog);
            }

            await _context.SaveChangesAsync();
        }

        public bool IsEmployeeRadiationDoseExceeded(int employee_id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == employee_id);

            var entrances = _context.EmployeeEntrance
                .Where(e => e.EmployeeId == employee_id && e.EntranceTime >= DateTime.UtcNow.AddHours(-24))
                .Include(e => e.Zone)
                .ToList();

            double radiation = 0.0d;

            foreach (var entrance in entrances)
            {
                radiation += CalculateRadiationDose(entrance.Zone.Radiation, CalculateDuration(entrance.EntranceTime, entrance.ExitTime));
            }

            if (radiation > 1_000_000.0)
            {
                return true;
            }

            return false;
        }

        public double CalculateDuration(DateTime entranceTime, DateTime? exitTime)
        {
            double duration = exitTime.HasValue
                ? (exitTime.Value - entranceTime).TotalMinutes
                : (DateTime.UtcNow - entranceTime).TotalMinutes;

            return double.Round(duration, 1, MidpointRounding.AwayFromZero);
        }

        public double CalculateRadiationDose(double radiation, double duration)
        {
            return double.Round(radiation * (duration / 60), 3, MidpointRounding.AwayFromZero);
        }

    }
}
