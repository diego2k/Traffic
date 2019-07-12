using UnityEngine;
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

    public static HoloLensResultMessage Results { get; set; }

    public static void ResetListner()
    {
        Results = new HoloLensResultMessage();
        IsTrafficDataValid = false;
        IsScenarioDataValid = false;
    }

#if WINDOWS_UWP
    public static StreamSocket socket;

    public void Start()
    {
        IsTrafficDataValid = false;
        IsScenarioDataValid = false;
        if (socket == null)
        {
            Task.Run(() => StartSocket());
        }
    }

    void Update()
    {
    }

    public async void StartSocket()
    {
        Debug.Log("Starting TCP listner ...");
        socket = new StreamSocket();

        // var test_ip = GetLocalIp();
        HostName serverHost = new HostName(ServerIP);
        try
        {
            await socket.ConnectAsync(serverHost, ConnectionPort.ToString());
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
                            //Debug.Log(cleanedString);
                            Envelope env = JsonUtility.FromJson<Envelope>(cleanedString);
                            if (env.type == typeof(HoloLensTraffic).Name)
                            {
                                if (!IsTrafficDataValid)
                                {
                                    Results.TrafficStartTicks = DateTime.Now.Ticks;
                                }

                                TrafficData = JsonUtility.FromJson<HoloLensTraffic>(env.content);
                                IsTrafficDataValid = true;
                            }
                            else if (env.type == typeof(ScenarioData).Name)
                            {
                                ScenarioData = JsonUtility.FromJson<ScenarioData>(env.content);
                                IsScenarioDataValid = true;
                                Results.SzenarioName = ScenarioData.Name;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        catch (System.Runtime.InteropServices.COMException e)
        {
            Debug.Log(e.Message);
        }
        Debug.Log("TCP listner closed.");
    }

    private static async Task SendDataToCLient(string data)
    {
        Debug.Log("Send Data:" + data);
        if (socket == null)
        {
            Debug.LogError("Socket is NULL!");
            return;
        }
        using (DataWriter writer = new DataWriter(socket.OutputStream))
        {
            writer.WriteString(data);
            await writer.StoreAsync();
            writer.DetachStream();
        }
    }

    public static async Task SendReadyForTraffic()
    {
        Envelope env = new Envelope()
        {
            content = JsonUtility.ToJson(new HoloLensStatusMessage()
            {
                readyForTraffic = true,
            }),
            type = typeof(HoloLensStatusMessage).Name
        };
        await SendDataToCLient(JsonUtility.ToJson(env));
    }

    public static async Task SendResults()
    {
        Envelope env = new Envelope()
        {
            content = JsonUtility.ToJson(Results),
            type = typeof(HoloLensResultMessage).Name
        };
        await SendDataToCLient(JsonUtility.ToJson(env));
    }

#endif
}
