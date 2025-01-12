using IOT.Data;
using MQTTnet;
using System.Text.Json;
using IOT.Data;
using IOT.Models;
using System.Text;
using System.Diagnostics;

namespace IOT.Mqtt
{
    public class MqttClient
    {
        private readonly AppDbContext _context;
        private IMqttClient _mqttClient;


        public MqttClient(AppDbContext context)
        {
            _context = context;

            var broker = "10.108.33.121";
            var port= 1883; // default mqtt port
            var clientId = "T0";


            var factory = new MqttClientFactory();

            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port) 
                .WithClientId(clientId)
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

                if (log != null)
                {
                    //TODO check if log.EmployeeId, log.ZoneId exists in the database
                    //TODO check if log.EmployeeId, log.ZoneId log.Timestamp are not null

                    var dbLog = _context.EmployeeEntrance
                        .FirstOrDefault(e => e.EmployeeId == log.EmployeeId && e.ExitTime == null);

                    if (log != null)
                    {
                        dbLog.ExitTime = log.Timestamp;
                        _context.EmployeeEntrance.Update(dbLog);
                    }
                    else {
                        var newLog = new EmployeeEntrance() {
                            EmployeeId = log.EmployeeId,
                            ZoneId = log.ZoneId,
                            EntranceTime = log.Timestamp,
                            ExitTime = null
                        };

                        _context.EmployeeEntrance.Add(newLog);
                    }


                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        public class EntranceLog
        {
            public int EmployeeId { get; set; }
            public int ZoneId { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
