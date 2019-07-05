using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using traffic.server.Data;
using traffic.server.Net;

namespace traffic.server.Manager
{
    class TrafficManager
    {
        private AsynchronousUPDListner _me;
        private List<AsynchronousUPDListner> _traffic = new List<AsynchronousUPDListner>();
        private AsynchronousSocketListener _tcpListner;
        private TrafficData _myPosition = null;

        public TrafficManager()
        {
            Initilize();
        }

        private void Initilize()
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    _me = new AsynchronousUPDListner();
                    _me.UdpDataReceived += SimulationMe; ;
                    _me.Start(5001, 104);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // TODO: do something
                }
            });
            th.Start();

            for (int i = 0; i < 18; i++)
            {
                Thread th1 = new Thread(() =>
                {
                    try
                    {
                        var l = new AsynchronousUPDListner();
                        l.UdpDataReceived += SimulationTraffic;
                        l.Start(5002 + i, 104);
                        _traffic.Add(l);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // TODO: do something
                    }
                });
                th1.Start();
                Thread.Sleep(100);
            }

            _tcpListner = new AsynchronousSocketListener();
            _tcpListner.TcpDataReceived += HoloLensData;
            _tcpListner.StartAsync(11000);
        }

        private void HoloLensData(object sender, TcpDataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void SimulationTraffic(object sender, UdpDataReceivedEventArgs e)
        {
            if (_myPosition == null) return;

            TrafficData trafficPosition = new TrafficData(e.Data);

            // TODO: Convert

            var traffic = JsonConvert.SerializeObject(new HoloLensTraffic()
            {
                X = 0,
                Y = 1,
                Z = 3,
                RotationX = 0,
                RotationY = 0,
                RotationZ = 0
            });
            _tcpListner.Send(traffic);
        }

        private void SimulationMe(object sender, UdpDataReceivedEventArgs e)
        {
            _myPosition = new TrafficData(e.Data);
        }
    }
}
