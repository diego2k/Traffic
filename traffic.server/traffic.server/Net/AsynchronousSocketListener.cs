using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace traffic.server.Net
{
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static Dictionary<string, string> dict = new Dictionary<string, string>();
        private static Socket handler;
        public event EventHandler<TcpDataReceivedEventArgs> TcpDataReceived;

        public AsynchronousSocketListener()
        {
        }

        public void StartAsync(int port)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    StartListening(port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            th.Start();
        }

        private void StartListening(int port)
        {
            //TODO: Parse input IP
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Console.WriteLine($"TCP listening at {ipAddress.ToString()}:{port}");

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            handler = listener.EndAccept(ar);

            var date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            dict.Add(date, "Client Connected");
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            //Socket handler2 = state.workSocket;
            try
            {
                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read   
                    // more data.  
                    try
                    {
                        content = state.sb.ToString();
                        Console.WriteLine(content);
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

                    if (content.IndexOf("<EOF>") > -1)
                    {
                        // All the data has been read from the   
                        // client. Display it on the console.  
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                            content.Length, content);
                        // Echo the data back to the client.  
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
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
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
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
