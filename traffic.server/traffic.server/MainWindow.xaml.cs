using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using traffic.server.Net;

namespace traffic.server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AsynchronousUPDListner _updListner;
        private AsynchronousSocketListener _tcpListner;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_updListner != null)
            {
                // TODO: kill it
                _updListner.UdpDataReceived -= _updListner_UdpDataReceived;
            }
            _updListner = new AsynchronousUPDListner();
            _updListner.UdpDataReceived += _updListner_UdpDataReceived;
            _updListner.StartAsync();

            if(_tcpListner != null)
            {
                // TODO: kill it
            }
            _tcpListner = new AsynchronousSocketListener();
            _tcpListner.StartAsync();
        }

        private void _updListner_UdpDataReceived(object sender, UdpDataReceivedEventArgs e)
        {
            Console.WriteLine(e.DataJson);
        }
    }
}
