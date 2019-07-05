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
using traffic.server.Manager;
using traffic.server.Net;

namespace traffic.server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrafficManager _trafficManager;

        public MainWindow()
        {
            InitializeComponent();
            _trafficManager = new TrafficManager();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
