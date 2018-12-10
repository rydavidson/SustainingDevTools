using Console.Common;
using Console.Modules.Configuration;
using rydavidson.Accela.Configuration.ConfigModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Console.ViewModel
{
    /// <summary>
    /// Interaction logic for DatabaseControl.xaml
    /// </summary>
    public partial class DatabaseControl : UserControl
    {
        public DatabaseControl(Window callingWindow)
        {
            InitializeComponent();
        }

        private void AADatabaseTextBox_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void SaveConfig(object sender, EventArgs e)
        {
            AVServerConfig config = new AVServerConfig()
            {
                AvDbHost = DatabaseServerTextBox.Text,
                AvDbName = AADatabaseTextBox.Text,
                AvJetspeedDbName = JetspeedDatabaseTextBox.Text,
                AvUser = AAUsernameTextBox.Text,
                AvJetspeedUser = JetspeedUsernameTextBox.Text,
                DatabaseType = DatabaseTypeComboBox.Text,
                Port = PortTextBox.Text
            };
            config.SetAvDatabasePassword(AAPasswordBox.Password);
            config.SetJetspeedDatabasePassword(JetspeedPasswordBox.Password);
            ConfigFacade configFacade = new ConfigFacade(new DirectoryInfo(Cache.Instance.Settings["env_base_dir"]));
            configFacade.SetDatabaseConfig(config, null);
        }
    }
}
