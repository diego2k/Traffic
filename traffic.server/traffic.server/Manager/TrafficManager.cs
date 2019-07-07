using GalaSoft.MvvmLight.Messaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using traffic.server.Data;
using traffic.server.Messages;
using traffic.server.Net;

namespace traffic.server.Manager
{
    class TrafficManager
    {
        private List<AsynchronousUPDListner> _traffic = new List<AsynchronousUPDListner>();
        private AsynchronousSocketListener _tcpListner;
        private TrafficData _myPosition = null;

        public TrafficManager()
        {
            Initilize();
        }

        private void Initilize()
        {
            for (int i = 1; i < 20; i++)
            {
                Thread th1 = new Thread(() =>
                {
                    int port = 5000 + i;
                    try
                    {
                        var l = new AsynchronousUPDListner();
                        l.UdpDataReceived += SimulationTraffic;
                        _traffic.Add(l);
                        Messenger.Default.Send(new PortStatusMessage()
                        {
                            Port = port,
                            IsRunning = true
                        });
                        l.Start(port, 104);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Messenger.Default.Send(new PortStatusMessage()
                        {
                            Port = port,
                            IsRunning = false
                        });
                    }
                });
                th1.Start();
                Thread.Sleep(100);
            }

            Thread th = new Thread(() =>
            {
                try
                {
                    _tcpListner = new AsynchronousSocketListener();
                    _tcpListner.TcpDataReceived += HoloLensData;
                    Messenger.Default.Send(new HoloLensStatusMessage()
                    {
                        Port = 11000,
                        IsRunning = true,
                        HoloLensConnected = false
                    });
                    _tcpListner.StartListening(11000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Messenger.Default.Send(new HoloLensStatusMessage()
                    {
                        Port = 11000,
                        IsRunning = false,
                        HoloLensConnected = false
                    });
                }
            });
            th.Start();
        }

        private void HoloLensData(object sender, TcpDataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void SimulationTraffic(object sender, UdpDataReceivedEventArgs e)
        {
            var listner = sender as AsynchronousUPDListner;
            if (listner == null) return;

            TrafficData trafficPosition = new TrafficData(e.Data);
            Messenger.Default.Send(new PortStatusMessage()
            {
                Port = listner.Port,
                IsRunning = true,
                Latitude = trafficPosition.Latitude,
                Longitude = trafficPosition.Longitude
            });

            if (listner.Port == 5001)
            {
                _myPosition = trafficPosition;
                return;
            }
            if (_myPosition == null) return;

            // TODO: Convert
            Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                {1,1,1,1},
                {1,2,3,4},
                {4,3,2,1}});
            Vector<double>[] nullspace = A.Kernel();

            // verify: the following should be approximately (0,0,0)
            var res = (A * (2 * nullspace[0] - 3 * nullspace[1]));


            var traffic = JsonConvert.SerializeObject(new HoloLensTraffic()
            {
                X = 0,
                Y = 1,
                Z = 3,
                RotationX = 0,
                RotationY = 0,
                RotationZ = 0
            });

            var env = new Envelope()
            {
                content = JsonConvert.SerializeObject(traffic),
                type = typeof(HoloLensTraffic).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
        }

        internal void SendScenario(ScenarioData scenario)
        {
            string bla = "";

            var a = JsonConvert.DeserializeObject<Envelope>(bla);

            var env = new Envelope()
            {
                content = JsonConvert.SerializeObject(scenario),
                type = typeof(ScenarioData).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
        }

    }
}
