using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccelaControlPanel.ViewModel
{
    /// <summary>
    /// Interaction logic for OverviewControl.xaml
    /// </summary>
    public partial class OverviewControl : UserControl
    {
        public string hostname { get; set; }
        public string installDir { get; set; }
        public string installedVersion { get; set; }
        public List<string> installedServices { get; set; }
        public string dbServer { get; set; }
        public string automationDB { get; set; }
        public string jetspeedDB { get; set; }
        public string installedServicesString { get; set; }

        public OverviewControl()
        {
            installedServices = new List<string>();
            hostname = "Unknown";
            installDir = "Unknown";
            installedVersion = "Unknown";
            dbServer = "Unknown";
            automationDB = "Unknown";
            jetspeedDB = "Unknown";
            installedServicesString = "Unknown";
            InitializeComponent();

        }
    }
}
