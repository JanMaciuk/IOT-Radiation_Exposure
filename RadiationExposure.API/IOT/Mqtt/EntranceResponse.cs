namespace IOT.Mqtt
{
    public class EntranceResponse
    {
        public string CardId { get; set; }
        public int ZoneId  { get; set; }
        public bool AccessGranted { get; set; }
    }
}
