﻿using UnityEngine;
using System;
#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Newtonsoft.Json;
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
    public static bool IsTrafficDataValid { get; set; }
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
            (Results.CompassTurn == ScenarioData.CompassTurn ? 1 : 0) +
            (int)(Results.ScanningPatternResult * 100);
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
            if(command.Text == "quit")
            {
#if WINDOWS_UWP
                SendResults().Wait();
                Windows.ApplicationModel.Core.CoreApplication.Exit();
#endif
            }

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
                    var bytesRead = await reader.LoadAsync(900);
                    string cleanedString = reader.ReadString(bytesRead);

                    try
                    {
                        //Debug.Log(cleanedString);
                        Envelope env = JsonConvert.DeserializeObject<Envelope>(cleanedString);
                        if (env.type == typeof(HoloLensTraffic).Name)
                        {
                            if (!IsTrafficDataValid)
                            {
                                Results.TrafficStartTicks = DateTime.Now.Ticks;
                            }

                            TrafficData = JsonConvert.DeserializeObject<HoloLensTraffic>(env.content);
                            IsTrafficDataValid = true;
                        }
                        else if (env.type == typeof(ScenarioData).Name)
                        {
                            ScenarioData = JsonConvert.DeserializeObject<ScenarioData>(env.content);
                            Debug.Log(env.content+ " " + ScenarioData.CompassTurn);

                            IsScenarioDataValid = true;
                            Results.SzenarioName = ScenarioData.Name;
                        }
                        else if (env.type == typeof(NetworkSpeechCommand).Name)
                        {
                            Debug.Log(env.content);
                            var netcmd = JsonConvert.DeserializeObject<NetworkSpeechCommand>(env.content);
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
            content = JsonConvert.SerializeObject(new HoloLensStatusMessage()
            {
                readyForTraffic = true,
            }),
            type = typeof(HoloLensStatusMessage).Name
        };
        await SendDataToCLient(JsonConvert.SerializeObject(env));
    }

    public static async Task SendResults()
    {
        Envelope env = new Envelope()
        {
            content = JsonConvert.SerializeObject(Results),
            type = typeof(HoloLensResultMessage).Name
        };
        await SendDataToCLient(JsonConvert.SerializeObject(env));
    }

#endif
}
