﻿using GalaSoft.MvvmLight.Messaging;
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

            var d_b = I_bg(_myPosition.Longitude, _myPosition.Latitude, r) * 
                      I_ge(_myPosition.YawAngle, _myPosition.PitchAngle, _myPosition.RollAngle) * 
                      d_e(trafficPosition.Longitude, trafficPosition.Latitude, r);

            var traffic = new HoloLensTraffic()
            {
                PosX = (float)d_b[1, 0],
                PosY = -(float)d_b[2, 0],
                PosZ = (float)d_b[0, 0],
                RotationX = 0,
                RotationY = 0,
                RotationZ = 0
            };
            Console.WriteLine($"{traffic.PosX} {traffic.PosY} {traffic.PosZ}");
            var env = new Envelope()
            {
                Content = JsonConvert.SerializeObject(traffic),
                Type = typeof(HoloLensTraffic).Name
            };
            _tcpListner.Send(JsonConvert.SerializeObject(env));
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

    }
}
