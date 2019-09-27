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

    public static int Points
    {
        get
        {
            return (Results.Collide == ScenarioData.Collide ? 1 : 0) +
            (ScenarioData.RightOfWay > 0 && Results.RightOfWay == ScenarioData.RightOfWay ? 1 : 0) +
            (Results.Turn == ScenarioData.Turn ? 1 : 0) +
            (Results.CompassTurn == ScenarioData.CompassTurn ? 1 : 0);
        }
    }

    public static void ResetListner()
    {
        Results = new HoloLensResultMessage();
        IsTrafficDataValid = false;
        IsScenarioDataValid = false;
    }

    private void SendNetworkSpeechCommand(NetworkSpeechCommand command)
    {
        try
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                HoloToolkit.Unity.InputModule.SpeechInputSource src = new HoloToolkit.Unity.InputModule.SpeechInputSource();
                HoloToolkit.Unity.InputModule.InputManager.Instance.RaiseSpeechKeywordPhraseRecognized(
                    src,
                    999,
                    UnityEngine.Windows.Speech.ConfidenceLevel.High,
                    TimeSpan.MinValue,
                    DateTime.Now,
                    new UnityEngine.Windows.Speech.SemanticMeaning[0],
                    command.Text);
                Debug.Log("SendNetworkSpeechCommand worked!");
            }, false);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

#if WINDOWS_UWP
    public static StreamSocket socket;

    public void Start()
    {
        IsTrafficDataValid = false;
        IsScenarioDataValid = false;
        if (socket == null)
        {
            Task.Run(async() =>
            {
                while (true)
                {
                    await StartSocket();
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
            });
        }
    }

    public async Task StartSocket()
    {
        Debug.Log("Starting TCP listner ...");
        socket = new StreamSocket();

        // var test_ip = GetLocalIp();
        HostName serverHost = new HostName(ServerIP);
        try
        {
            await socket.ConnectAsync(serverHost, ConnectionPort.ToString());
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
                            Debug.Log(env.content+ " " + ScenarioData.CompassTurn);

                            IsScenarioDataValid = true;
                            Results.SzenarioName = ScenarioData.Name;
                        }
                        else if (env.type == typeof(NetworkSpeechCommand).Name)
                        {
                            Debug.Log(env.content);
                            var netcmd = JsonUtility.FromJson<NetworkSpeechCommand>(env.content);
                            SendNetworkSpeechCommand(netcmd);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
        }
        catch (System.Runtime.InteropServices.COMException e)
        {
            Debug.Log(e.Message);
        }
        catch (Exception e)
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
