﻿using UnityEngine;
using Newtonsoft.Json;
using System;
#if WINDOWS_UWP
using Windows.Networking.Connectivity;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

/// <summary>
/// Refactoring of the whole class is necessary!
/// </summary>
public class TcpListner : MonoBehaviour
{

    [Tooltip("The IPv4 Address of the machine running the Unity editor.")]
    public string ServerIP;

    [Tooltip("The connection port on the machine to use.")]
    public int ConnectionPort = 11000;

    public static HoloLensTraffic TrafficData { get; private set; }
    public static bool IsTrafficDataValid { get; private set; }
    public static ScenarioData ScenarioData { get; private set; }
    public static bool IsScenarioDataValid { get; private set; } 

#if WINDOWS_UWP
    public static StreamSocket socket;
    private static string PORT = "11000";
    //uni
    //private static string ip = "192.168.1.145";
    //home
    //private static string ip = "192.168.8.102";
    //work
    private static string IP = "192.168.11.41";
    //sim
    //private static string ip = "192.168.8.38";
    //voi
    //private static string ip = "192.168.1.102";

    public void Start()
    {
        IsTrafficDataValid = false;
        IsScenarioDataValid = false;
        if (socket == null)
        {
            Task.Run(() => StartSocket());
        }
    }

    public async void StartSocket()
    {
        Console.WriteLine("Starting TCP listner ...");
        socket = new StreamSocket();

        // var test_ip = GetLocalIp();
        HostName serverHost = new HostName(IP);
        try
        {
            await socket.ConnectAsync(serverHost, PORT);
            try
            {
                using (DataReader reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.ReadAhead;

                    while (true)
                    {
                        IAsyncOperation<uint> taskLoad = reader.LoadAsync(900);
                        //taskLoad.AsTask().Wait();
                        await taskLoad.AsTask().ConfigureAwait(false);
                        var bytesRead = taskLoad.GetResults();
                        if (bytesRead != 900)
                        {
                            var x = 0;
                        }
                        string cleanedString = reader.ReadString(bytesRead);

                        try
                        {
                            Console.WriteLine(cleanedString);
                            Envelope env = JsonConvert.DeserializeObject<Envelope>(cleanedString);
                            if (env.type == typeof(HoloLensTraffic).Name)
                            {
                                TrafficData = JsonConvert.DeserializeObject<HoloLensTraffic>(env.content);
                                IsTrafficDataValid = true;
                            }

                            else if (env.type == typeof(ScenarioData).Name)
                            {
                                ScenarioData = JsonConvert.DeserializeObject<ScenarioData>(env.content);
                                IsScenarioDataValid = true;
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        catch (System.Runtime.InteropServices.COMException e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("TCP listner closed.");
    }

    void Update()
    {
    }

    public static async void SendDataToCLient(string data)
    {
        using (DataWriter writer = new DataWriter(socket.OutputStream))
        {
            writer.WriteString(data);
            await writer.StoreAsync();
            writer.DetachStream();
        }
    }

#endif
}
