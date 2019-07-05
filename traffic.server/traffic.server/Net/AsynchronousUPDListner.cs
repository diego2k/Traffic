using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace traffic.server.Net
{
    public class UdpDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public DateTime Time { get; set; }
    }

    public class AsynchronousUPDListner
    {
        public event EventHandler<UdpDataReceivedEventArgs> UdpDataReceived;

        public int Port { get; set; }

        public bool IsRunning { get; set; } = true;

        public void Start(int port, int size)
        {
            Port = port;
            var endpoint = new IPEndPoint(IPAddress.Any, port);
            UdpClient receivingUdpClient = new UdpClient(endpoint);
            Console.WriteLine($"UPD listinging at {endpoint.ToString()}");

            receivingUdpClient.Client.ReceiveBufferSize = size;

            receivingUdpClient.EnableBroadcast = true;
            receivingUdpClient.Client.EnableBroadcast = true;

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (IsRunning)
            {
                try
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref endpoint);

                    //Console.WriteLine("data received");
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    //Console.WriteLine(returnData);

                    OnUdpDataReceived(new UdpDataReceivedEventArgs()
                    {
                        Data = receiveBytes,
                        Time = DateTime.Now,
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        protected virtual void OnUdpDataReceived(UdpDataReceivedEventArgs e)
        {
            EventHandler<UdpDataReceivedEventArgs> handler = UdpDataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
