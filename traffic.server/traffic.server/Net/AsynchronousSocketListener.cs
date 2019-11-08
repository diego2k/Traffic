using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using traffic.server.Messages;

namespace traffic.server.Net
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class TcpDataReceivedEventArgs : EventArgs
    {
        public string Data { get; set; }
        public DateTime Time { get; set; }
    }

    public class AsynchronousSocketListener
    {
        private int _port = 0;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static Socket handler;
        public event EventHandler<TcpDataReceivedEventArgs> TcpDataReceived;

        public AsynchronousSocketListener()
        {
        }

        public void StartListening(int port)
        {
            _port = port;
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

            Console.WriteLine($"TCP listening at {ipAddress.ToString()}:{_port}");

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Messenger.Default.Send(new InternalHoloLensStatusMessage()
            {
                Port = _port,
                IsRunning = true,
                HoloLensConnected = true
            });
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            try
            {
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    try
                    {
                        content = state.sb.ToString();
                        OnTcpDataReceived(new TcpDataReceivedEventArgs()
                        {
                            Data = content,
                            Time = DateTime.Now,
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // Data is less than 1024 bytes so 
                    state = new StateObject();
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                Messenger.Default.Send(new InternalHoloLensStatusMessage()
                {
                    Port = _port,
                    IsRunning = true,
                    HoloLensConnected = false
                });
            }
        }

        public void Send(string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            var missingBytes = 900 - byteData.Length;
            Array.Resize(ref byteData, byteData.Length + missingBytes);
            var str = System.Text.Encoding.Default.GetString(byteData);

            if (handler != null)
            {
                bool ConnectionClosed = handler.Poll(1000, SelectMode.SelectRead);
                if (!ConnectionClosed)
                {
                    // Begin sending the data to the remote device.  
                    handler.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), handler);
                    Console.WriteLine(data);
                }
            }
        }

        private void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
            }
            catch
            {
                Console.Write("Client Disconnected");
            }
        }

        protected virtual void OnTcpDataReceived(TcpDataReceivedEventArgs e)
        {
            EventHandler<TcpDataReceivedEventArgs> handler = TcpDataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
