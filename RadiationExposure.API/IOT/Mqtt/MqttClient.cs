using IOT.Data;
using MQTTnet;
using System.Text.Json;
using IOT.Models;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Options;


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

                    await _mqttClient.SubscribeAsync("iot/entrance_logs");
                    Debug.WriteLine("Subscribed to topic: iot/entrance_logs");

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
    }
}
