using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using traffic.server.Data;
using traffic.server.Manager;
using traffic.server.Messages;

namespace traffic.server.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private TrafficManager _trafficManager;
        private SolidColorBrush _statusHoloLensColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush _HoloLensConnectedColor = new SolidColorBrush(Colors.Red);
        private string _holoLensPort = string.Empty;
        private ScenarioData _selectedScenario = null;

        public SolidColorBrush[] StatusColor { get; set; } = new SolidColorBrush[19];

        public string[] Port { get; set; } = new string[19];

        public string[] Latitude { get; set; } = new string[19];

        public string[] Longitude { get; set; } = new string[19];

        public SolidColorBrush StatusHoloLensColor
        {
            get { return _statusHoloLensColor; }
            set { Set(ref _statusHoloLensColor, value); }
        }

        public string HoloLensPort
        {
            get { return _holoLensPort; }
            set { Set(ref _holoLensPort, value); }
        }

        public SolidColorBrush HoloLensConnectedColor
        {
            get { return _HoloLensConnectedColor; }
            set { Set(ref _HoloLensConnectedColor, value); }
        }

        public List<ScenarioData> ScenarioData { get; set; }

        public ScenarioData SelectedScenario
        {
            get { return _selectedScenario; }
            set { Set(ref _selectedScenario, value); SendScenarioCommand.RaiseCanExecuteChanged(); }
        }

        public RelayCommand SendScenarioCommand { get; set; }

        public MainViewModel()
        {
            try
            {
                using (var f = File.OpenRead("ScenarioData.json"))
                {
                    using (var r = new StreamReader(f))
                    {
                        ScenarioData = JsonConvert.DeserializeObject<List<ScenarioData>>(r.ReadToEnd());
                    }
                }
            }
            catch
            {
                MessageBox.Show("ScenarioData.json corrupted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            for (int i = 0; i < StatusColor.Length; i++) { StatusColor[i] = new SolidColorBrush(Colors.Red); }
            SendScenarioCommand = new RelayCommand(SendScenario, CanSendScenario);

            Messenger.Default.Register<PortStatusMessage>(this, PortStatus);
            Messenger.Default.Register<InternalHoloLensStatusMessage>(this, HoloLensStatus);

            _trafficManager = new TrafficManager();
        }

        private bool CanSendScenario()
        {
            return SelectedScenario != null;
        }

        private void SendScenario()
        {
            _trafficManager.SendScenario(SelectedScenario);
            SelectedScenario = null;
        }

        private void PortStatus(PortStatusMessage data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                StatusColor[data.Port - 5001] = data.IsRunning ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                Port[data.Port - 5001] = data.Port.ToString();
                Latitude[data.Port - 5001] = data.Latitude.ToString();
                Longitude[data.Port - 5001] = data.Longitude.ToString();
                RaisePropertyChanged("Port");
                RaisePropertyChanged("Latitude");
                RaisePropertyChanged("Longitude");
                RaisePropertyChanged("StatusColor");
            }));
        }

        private void HoloLensStatus(InternalHoloLensStatusMessage status)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                StatusHoloLensColor = status.IsRunning ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                HoloLensConnectedColor = status.HoloLensConnected ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                HoloLensPort = status.Port.ToString();
            }));
        }
    }
}