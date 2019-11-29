using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading;
#if WINDOWS_UWP
using Windows.Networking.Connectivity;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace traffic.testclient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string ServerIP = "127.0.0.1";

        public int ConnectionPort = 11000;

        public static HoloLensTraffic TrafficData { get; private set; }
        public static bool IsTrafficDataValid { get; private set; }
        public static ScenarioData ScenarioData { get; private set; }
        public static bool IsScenarioDataValid { get; private set; }

        public static HoloLensResultMessage Results { get; set; }

        public MainPage()
        {
            Start();
            Results = new HoloLensResultMessage();
            this.InitializeComponent();
        }

#if WINDOWS_UWP
        private static StreamSocket socket;
        private string _data = string.Empty;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        public void Start()
        {
            IsTrafficDataValid = false;
            IsScenarioDataValid = false;
            if (socket == null)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await StartSocket();
                        await Task.Delay(new TimeSpan(0, 0, 10));
                    }
                });

                Task.Run(() => { WorkQueue(); });
            }
        }

        private void WorkQueue()
        {
            while (true)
            {
                try
                {
                    _waitHandle.WaitOne();
                    if (string.IsNullOrEmpty(_data)) continue;
                    Envelope env = JsonConvert.DeserializeObject<Envelope>(_data);
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

                        IsScenarioDataValid = true;
                        Results.SzenarioName = ScenarioData.Name;
                    }
                    else if (env.type == typeof(NetworkSpeechCommand).Name)
                    {
                        var netcmd = JsonConvert.DeserializeObject<NetworkSpeechCommand>(env.content);
                        SendNetworkSpeechCommand(netcmd);
                    }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine(e.Message);
                }
            }
        }

        private async Task StartSocket()
        {
            Debug.WriteLine("Starting TCP listner ...");
            socket = new StreamSocket();

            // var test_ip = GetLocalIp();
            HostName serverHost = new HostName(ServerIP);
            try
            {
                await socket.ConnectAsync(serverHost, ConnectionPort.ToString());
                using (DataReader reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    while (true)
                    {
                        var bytesRead = await reader.LoadAsync(1024);

                        var tmp = reader.ReadString(bytesRead);
                        if (string.IsNullOrEmpty(tmp)) continue;
                        _data = tmp.Replace("\0", "");

                        _waitHandle.Set();
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                //Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }
            Debug.WriteLine("TCP listner closed.");
        }

        private static async Task SendDataToCLient(string data)
        {
            Debug.WriteLine("Send Data:" + data);
            if (socket == null)
            {
                Debug.WriteLine("Socket is NULL!");
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
                content = JsonConvert.DeserializeObject(new HoloLensStatusMessage()
                {
                    readyForTraffic = true,
                }),
                type = typeof(HoloLensStatusMessage).Name
            };
            await SendDataToCLient(JsonConvert.DeserializeObject(env));
        }

        public static async Task SendResults()
        {
            Envelope env = new Envelope()
            {
                content = JsonConvert.DeserializeObject(Results),
                type = typeof(HoloLensResultMessage).Name
            };
            await SendDataToCLient(JsonConvert.DeserializeObject(env));
        }

#endif
    }
}
