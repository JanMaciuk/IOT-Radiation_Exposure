using IOT.Data;
using MQTTnet;
using System.Text.Json;
using IOT.Models;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Serilog;
using IOT.Dto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Protocol;


namespace IOT.Mqtt
{
    public class MqttClient
    {
        private readonly AppDbContext _context;
        private IMqttClient _mqttClient;
        private MqttSettings _settings;


        public MqttClient(AppDbContext context, IOptions<MqttSettings> settings)
        {
            _context = context;
            //_settings = settings.Value;
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));


            var factory = new MqttClientFactory();

            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_settings.BrokerAddress, _settings.Port) 
                .WithClientId(_settings.ClientId)
                .WithCleanSession()
                .Build();
            //.WithCredentials(username, password)

            //mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

            //_mqttClient.ConnectAsync(options).Wait();
            //_mqttClient.SubscribeAsync("iot/entrance_logs").Wait();

            ConnectAndSubscribeAsync(options).Wait();
        }

        private async Task ConnectAndSubscribeAsync(MqttClientOptions options)
        {
            // Opional while loop to keep trying to connect
            try
            {
                var connectResult = await _mqttClient.ConnectAsync(options);

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Debug.WriteLine("Connected to MQTT successfully.");

                    await _mqttClient.SubscribeAsync(_settings.Topic);
                    Debug.WriteLine($"Subscribed to topic: {_settings.Topic}");

                    _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
                }
                else
                {
                    Debug.WriteLine($"Failed to connect broker: {connectResult.ReasonString}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error xd: {ex.Message}");
            }
        }


        private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                var log = JsonSerializer.Deserialize<EntranceLog>(payload);

                if (log == null || string.IsNullOrEmpty(log.CardId))
                {
                    Debug.WriteLine("Invalid or null log received.");
                    return;
                }

                await ProcessMessage(log);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private async Task ProcessMessage(EntranceLog log) {

            var employee = _context.Employees.FirstOrDefault(e => e.Card == log.CardId);
            var zone = _context.Zones.FirstOrDefault(z => z.Id == log.ZoneId);

            if (employee == null)
            {
                Debug.WriteLine($"Employee not found for CardId: {log.CardId}");
                return;
            }

            if (zone == null)
            {
                Debug.WriteLine($"Zone not found for ZoneId: {log.ZoneId}");
                return;
            }
            var exceededRadiation = IsEmployeeRadiationDoseExceeded(employee.Id);
            var response = new EntranceResponse
            {
                CardId = log.CardId,
                ZoneId = log.ZoneId,
                AccessGranted = !exceededRadiation
            };

            var jsonPayload = JsonSerializer.Serialize(response);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_settings.Topic)
                .WithPayload(Encoding.UTF8.GetBytes(jsonPayload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce) //
                .WithRetainFlag() //
                .Build();

            await _mqttClient.PublishAsync(message);

            var existingLog = _context.EmployeeEntrance
               .FirstOrDefault(e => e.EmployeeId == employee.Id && e.ExitTime == null);

            if (existingLog != null)
            {
                existingLog.ExitTime = log.Timestamp;
                _context.EmployeeEntrance.Update(existingLog);
            }
            else
            {
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

        public bool IsEmployeeRadiationDoseExceeded(int employee_id){
            var employee = _context.Employees.FirstOrDefault(e => e.Id == employee_id);

            var entrances = _context.EmployeeEntrance
                .Where(e => e.EmployeeId == employee_id && e.EntranceTime >= DateTime.UtcNow.AddHours(-24))
                .Include(e => e.Zone)
                .ToList();

            double radiation = 0.0d; 

            foreach (var entrance in entrances) {
                radiation += CalculateRadiationDose(entrance.Zone.Radiation,CalculateDuration(entrance.EntranceTime, entrance.ExitTime));
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
