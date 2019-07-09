namespace traffic.server.Messages
{
    public class InternalHoloLensStatusMessage
    {
        public int Port { get; set; }
        public bool IsRunning { get; set; }
        public bool HoloLensConnected { get; set; }
    }
}
