using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using traffic.server.Messages;

namespace traffic.server.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private SolidColorBrush _statusHoloLensColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush _HoloLensConnectedColor = new SolidColorBrush(Colors.Red);
        private string _holoLensPort = string.Empty;

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

        public MainViewModel()
        {
            for (int i = 0; i < StatusColor.Length; i++) { StatusColor[i] = new SolidColorBrush(Colors.Red); }
            Messenger.Default.Register<PortStatusMessage>(this, PortStatus);
            Messenger.Default.Register<HoloLensStatusMessage>(this, HoloLensStatus);
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

        private void HoloLensStatus(HoloLensStatusMessage status)
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