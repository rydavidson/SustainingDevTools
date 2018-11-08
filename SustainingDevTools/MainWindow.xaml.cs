using SustainingDevTools.ViewModel;
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

namespace SustainingDevTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestControl testControl;
        OverviewControl overviewControl;
        GeneralControl generalControl;
        DatabaseControl databaseControl;
        SettingsControl settingsControl;
        AdvancedControl advancedControl;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            testControl = new TestControl(this);
            overviewControl = new OverviewControl(this);
            generalControl = new GeneralControl(this);
            databaseControl = new DatabaseControl(this);
            settingsControl = new SettingsControl(this);
            advancedControl = new AdvancedControl(this);

            contentControl.Content = generalControl;
        }

        private void overviewButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = overviewControl;
        }

        private void generalButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = generalControl;
        }

        private void debuggingButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = testControl;
        }

        private void advancedButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = advancedControl;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = settingsControl;
        }

        private void databaseButton_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = databaseControl;
        }
    }
}
