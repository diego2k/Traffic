namespace traffic.server.Messages
{
    public class PortStatusMessage
    {
        public int Port { get; set; }
        public bool IsRunning { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
