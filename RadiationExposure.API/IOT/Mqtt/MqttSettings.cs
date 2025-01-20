namespace IOT.Mqtt
{
    public class MqttSettings
    {
        public string BrokerAddress { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string TopicLogs { get; set; }
        public string TopicResponse { get; set; }
    }
}
