using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using traffic.server.Data;

namespace traffic.server.Net
{
    public class UdpDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public string DataJson { get; set; }
        public DateTime Time { get; set; }
    }

    public class AsynchronousUPDListner
    {
        public event EventHandler<UdpDataReceivedEventArgs> UdpDataReceived;

        public void StartAsync()
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    ListenSimulator(5259, "aircraft", 124);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            th.Start();
        }

        private void ListenSimulator(int port, string type, int size)
        {
            var endpoint = new IPEndPoint(IPAddress.Any, port);
            UdpClient receivingUdpClient = new UdpClient(endpoint);
            Console.WriteLine($"UPD listinging at {endpoint.ToString()}");

            receivingUdpClient.Client.ReceiveBufferSize = size;

            receivingUdpClient.EnableBroadcast = true;
            receivingUdpClient.Client.EnableBroadcast = true;

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            AircraftKinematicsData aircraftKinematicsData = new AircraftKinematicsData();
            //CriteriaData criteriaData = new CriteriaData();

            while (true)
            {
                //Creates a UdpClient for reading incoming data.

                //Creates an IPEndPoint to record the IP Address and port number of the sender. 
                // The IPEndPoint will allow you to read datagrams sent from any source.
                try
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref endpoint);

                    //Console.WriteLine("data received");
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    //Console.WriteLine(returnData);

                    if (type == "aircraft")
                    {
                        OnUdpDataReceived(new UdpDataReceivedEventArgs()
                        {
                            Data = receiveBytes,
                            DataJson = aircraftKinematicsData.ConvertData(receiveBytes),
                            Time = DateTime.Now,
                        });
                    }
                    else if (type == "criteria")
                    {
                        //criteriaData.ConvertData(receiveBytes);
                    }
                    else if (type == "speed")
                    {
                        //Console.WriteLine(receiveBytes);
                        //criteriaData.ConvertSpeedData(receiveBytes);
                    }
                    else
                    {
                        aircraftKinematicsData.checkSimulationRunning(receiveBytes);
                    }
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
