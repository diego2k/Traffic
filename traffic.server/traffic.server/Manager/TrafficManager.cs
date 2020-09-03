using GalaSoft.MvvmLight.Messaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
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
        int count = 0;
        const string PATH = "Resutls.csv";

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
                    Messenger.Default.Send(new InternalHoloLensStatusMessage()
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
                    Messenger.Default.Send(new InternalHoloLensStatusMessage()
                    {
                        Port = 11000,
                        IsRunning = false,
                        HoloLensConnected = false
                    });
                }
            });
            th.Start();
            Thread.Sleep(100);

            //Thread th2 = new Thread(() =>
            //{
            //    int port = 5423;
            //    try
            //    {
            //        var l = new AsynchronousUPDListner();
            //        l.UdpDataReceived += AircraftControls;
            //        _traffic.Add(l);
            //        l.Start(port, 104);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //});
            //th2.Start();

            Thread th3 = new Thread(() =>
            {
                int port = 5425;
                try
                {
                    var l = new AsynchronousUPDListner();
                    l.UdpDataReceived += CenterConsoleOutputData;
                    _traffic.Add(l);
                    l.Start(port, 14);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            th3.Start();
        }

        private void HoloLensData(object sender, TcpDataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            var env = JsonConvert.DeserializeObject(e.Data, typeof(Envelope)) as Envelope;
            if (env.Type == typeof(HoloLensStatusMessage).Name)
            {
                var status = JsonConvert.DeserializeObject(env.Content, typeof(HoloLensStatusMessage)) as HoloLensStatusMessage;
                if (status.readyForTraffic)
                    MessageBox.Show("User is ready for traffic! Choose the scenario in the DataPlayer and click Play.",
                        "Ready", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (env.Type == typeof(HoloLensResultMessage).Name)
            {
                var result = JsonConvert.DeserializeObject(env.Content, typeof(HoloLensResultMessage)) as HoloLensResultMessage;

                if (!File.Exists(PATH))
                {
                    using (StreamWriter sw = File.CreateText(PATH))
                    {
                        sw.WriteLine("SzenarioName\tCollide\tRightOfWay\tTurnRight\tCompassTurnRight\tTrafficStartTime\tCallDecidedTime\tAttempts\tScanningPatternResult\tScanningPatternIndividualResult");
                    }
                }
                using (StreamWriter sw = File.AppendText(PATH))
                {
                    sw.WriteLine($"{result.SzenarioName}\t{result.Collide}\t{result.RightOfWay}\t{result.Turn}\t{result.CompassTurn}\t{result.TrafficStartTicks}\t{result.CallDecidedTicks}\t{result.NumberOfAttempts}\t{result.ScanningPatternResult}\t{result.ScanningPatternIndividualResult}");
                }
            }
        }

        internal void SendScenario(ScenarioData scenario)
        {
            var env = new Envelope()
            {
                Content = JsonConvert.SerializeObject(scenario),
                Type = typeof(ScenarioData).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
        }

        internal void SendSpeechCommand(NetworkSpeechCommand speechCommand)
        {
            var env = new Envelope()
            {
                Content = JsonConvert.SerializeObject(speechCommand),
                Type = typeof(NetworkSpeechCommand).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
        }

        private void SimulationTraffic(object sender, UdpDataReceivedEventArgs e)
        {
            var listner = sender as AsynchronousUPDListner;
            if (listner == null) return;

            TrafficData trafficPosition = new TrafficData(e.Data);
            if (count % 10 == 0)
            {
                Messenger.Default.Send(new PortStatusMessage()
                {
                    Port = listner.Port,
                    IsRunning = true,
                    Latitude = trafficPosition.Latitude,
                    Longitude = trafficPosition.Longitude
                });
            }

            if (listner.Port == 5001)
            {
                _myPosition = trafficPosition;
                return;
            }
            if (_myPosition == null) return;

            var pos = CalculateTarget(_myPosition, trafficPosition);

            var traffic = new HoloLensTraffic()
            {
                PosX = pos.x,
                PosY = pos.y,
                PosZ = pos.z,
                RotationX = pos.rotationX,
                RotationY = pos.rotationY,
                RotationZ = pos.rotationZ
            };
            // Console.WriteLine($"{traffic.PosX} {traffic.PosY} {traffic.PosZ}");
            var env = new Envelope()
            {
                Content = JsonConvert.SerializeObject(traffic),
                Type = typeof(HoloLensTraffic).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
        }

        private void AircraftControls(object sender, UdpDataReceivedEventArgs e)
        {
            AircraftControlsData controlData = new AircraftControlsData(e.Data);
            if (controlData.APDisengageButton)
            {
                SendSpeechCommand(new NetworkSpeechCommand() { Text = "decided" });
            }
        }

        private void CenterConsoleOutputData(object sender, UdpDataReceivedEventArgs e)
        {
            TUG2SimCenterConsoleOutputData controlData = new TUG2SimCenterConsoleOutputData(e.Data);
            if (controlData.GoAroundButton)
            {
                SendSpeechCommand(new NetworkSpeechCommand() { Text = "decided" });
            }
        }

        private (float x, float y, float z, float rotationX, float rotationY, float rotationZ) CalculateTarget(TrafficData me, TrafficData traffic)
        {
            Func<double, double, double, Matrix<double>> I_eg = delegate (double longitude, double latitude, double radius)
            {
                return DenseMatrix.OfArray(new double[,] {
                {-Math.Cos(longitude)*Math.Sin(latitude),  -Math.Sin(longitude), -Math.Cos(longitude)*Math.Cos(latitude), radius*Math.Cos(longitude)*Math.Cos(latitude)},
                {-Math.Sin(longitude)*Math.Sin(longitude),  Math.Cos(longitude), -Math.Sin(longitude)*Math.Cos(latitude), radius*Math.Sin(longitude)*Math.Cos(latitude)},
                { Math.Cos(latitude),                       0,                   -Math.Sin(latitude),                     radius*Math.Sin(latitude)},
                { 0,                                        0,                   0,                                       1} });
            };

            Func<double, double, double, Matrix<double>> I_gb = delegate (double yaw, double pitch, double roll)
            {
                return DenseMatrix.OfArray(new double[,] {
                { Math.Cos(yaw)*Math.Cos(pitch), Math.Cos(yaw)*Math.Sin(pitch)*Math.Sin(roll) - Math.Sin(yaw)*Math.Cos(roll), Math.Cos(yaw)*Math.Sin(pitch)*Math.Cos(roll) + Math.Sin(yaw)*Math.Sin(roll), 0},
                { Math.Sin(yaw)*Math.Cos(pitch), Math.Sin(yaw)*Math.Sin(pitch)*Math.Sin(roll) + Math.Cos(yaw)*Math.Cos(roll), Math.Sin(yaw)*Math.Sin(pitch)*Math.Cos(roll) - Math.Cos(yaw)*Math.Sin(roll), 0},
                {-Math.Sin(pitch),               Math.Cos(pitch)*Math.Sin(roll),                                              Math.Cos(pitch)*Math.Cos(roll),                                              0},
                { 0,                             0,                                                                           0,                                                                           1} });
            };

            Func<double, double, double, Matrix<double>> I_ge = delegate (double longitude, double latitude, double radius)
            {
                return DenseMatrix.OfArray(new double[,] {
                {-Math.Cos(longitude)*Math.Sin(latitude), -Math.Sin(longitude)*Math.Sin(latitude),  Math.Cos(latitude), 0},
                {-Math.Sin(longitude),                     Math.Cos(longitude),                     0,                  0},
                {-Math.Cos(longitude)*Math.Cos(latitude), -Math.Sin(longitude)*Math.Cos(latitude), -Math.Sin(latitude), radius},
                { 0,                                       0,                                       0,                  1} });
            };

            Func<double, double, double, Matrix<double>> I_bg = delegate (double yaw, double pitch, double roll)
            {
                return DenseMatrix.OfArray(new double[,] {
                {Math.Cos(yaw)*Math.Cos(pitch),                                               Math.Sin(yaw)*Math.Cos(pitch),                                              -Math.Sin(pitch),                0},
                {Math.Cos(yaw)*Math.Sin(pitch)*Math.Sin(roll) - Math.Sin(yaw)*Math.Cos(roll), Math.Sin(yaw)*Math.Sin(pitch)*Math.Sin(roll) + Math.Cos(yaw)*Math.Cos(roll), Math.Cos(pitch)*Math.Sin(roll), 0},
                {Math.Cos(yaw)*Math.Sin(pitch)*Math.Cos(roll) + Math.Sin(yaw)*Math.Sin(roll), Math.Sin(yaw)*Math.Sin(pitch)*Math.Cos(roll) - Math.Cos(yaw)*Math.Sin(roll), Math.Cos(pitch)*Math.Cos(roll), 0},
                { 0,                                                                          0,                                                                           0,                              1} });
            };

            Func<double, double, double, Matrix<double>> d_e = delegate (double longitude, double latitude, double radius)
            {
                return DenseMatrix.OfArray(new double[,] {
                {radius*Math.Cos(longitude)*Math.Cos(latitude)},
                {radius*Math.Sin(longitude)*Math.Cos(latitude)},
                {radius*Math.Sin(latitude)},
                {1} });
            };

            const double r = 6355707;

            var I_eb = I_eg(traffic.Longitude, traffic.Latitude, r) * I_gb(traffic.YawAngle, traffic.PitchAngle, traffic.RollAngle);

            var I_bg_x_I_ge = I_bg(me.YawAngle, me.PitchAngle, me.RollAngle) *
                              I_ge(me.Longitude, me.Latitude, r);

            var D_b = I_bg_x_I_ge * d_e(traffic.Longitude, traffic.Latitude, r);

            var I_bb = I_bg_x_I_ge * I_eb;

            float rotY = RadianToDegree(Math.Atan2(I_bb[1, 0], I_bb[0, 0]));
            float rotX = RadianToDegree(Math.Atan2(I_bb[2, 1], I_bb[2, 2]));
            float rotZ = RadianToDegree(Math.Asin(-I_bb[2, 0]));

            return ((float)D_b[1, 0], -(float)D_b[2, 0], (float)D_b[0, 0], rotX, rotY, rotZ);
        }

        private static float RadianToDegree(double angle)
        {
            return (float) (angle * (180.0 / Math.PI));
        }
    }
}
