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
using AccelaControlPanel.ViewModel;

namespace AccelaControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OverviewControl overview;
        public MainWindow()
        {
            InitializeComponent();
            overview = new OverviewControl();
            this.contentControl.Content = overview;
            Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {

        }    


        private void InitializeOverview(OverviewControl control)
        {
            control.hostname = "Test";
        }
    }
}
