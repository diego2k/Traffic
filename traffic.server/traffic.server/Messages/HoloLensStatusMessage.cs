namespace traffic.server.Messages
{
    public class HoloLensStatusMessage
    {
        public int Port { get; set; }
        public bool IsRunning { get; set; }
        public bool HoloLensConnected { get; set; }
    }
}
