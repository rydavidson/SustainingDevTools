using NLog;
using SustainingDevTools.Common;
using SustainingDevTools.Modules.Git;
using SustainingDevTools.Modules.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace SustainingDevTools.ViewModel
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl
    {
        SettingsService settingsService;
        public Cache cache { get; set; }
        Window mainWindow;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DefaultLogFile = "Console.log";

        public TestControl(Window parent)
        {
            InitializeComponent();
            mainWindow = parent;
            Init();
        }

        #region init

        private void Init()
        {
            cache = Cache.Instance;
            loadingSpinner.Spin = false;
            loadingSpinner.Visibility = Visibility.Hidden;
            DataContext = this;
            settingsService = new SettingsService();
            LoadSettings();
            //InitLogger();
        }

        private void InitLogger(string _logFile = DefaultLogFile)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _logFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        #endregion

        #region business

        public void LoadSettings()
        {
            if (cache.Settings is null)
                Task.WaitAll(settingsService.ReloadSettings());

            Dictionary<string, string> settings = cache.Settings;
            passwordTextBox.Password = settings["git_password"];
            PopulateRepos();

        }

        #endregion

        #region ui

        private void PopulateRepos()
        {
            repoComboBox.Items.Clear();

            foreach (var repo in cache.Content.repositories)
            {
                repoComboBox.Items.Add(repo.name);
            }
        }

        private void PopulateBranches(dynamic repo)
        {
            branchComboBox.Items.Clear();

            foreach (var branch in repo.branches)
            {
                branchComboBox.Items.Add(branch.branch);
            }
        }

        #endregion

        #region events

        private void repoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var repo in cache.Content.repositories)
            {
                if (repo.name == repoComboBox.SelectedItem.ToString())
                {
                    PopulateBranches(repo);
                }
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            cache.Settings["git_password"] = SecureUtils.Protect(passwordTextBox.Password);
            await Task.Run(() => settingsService.WriteSettingsToFile());
        }

        private void loadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private async void GitButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;

            GitAdapter git = new GitAdapter();
            Action<int, int> statusAction = new Action<int, int>((completed, total) => ProgressBarAction(completed, total));
            Action<string, bool> spinnerAction = new Action<string, bool>((actionName, isRunning) => SpinnerAction(actionName, isRunning));

            foreach (var repo in cache.Content.repositories)
            {
                if (repo.name == repoComboBox.SelectedItem.ToString())
                {
                    try
                    {
                        string targetdir = !string.IsNullOrWhiteSpace(cache.Settings["repo_base_dir"]) ? cache.Settings["repo_base_dir"] : Environment.CurrentDirectory;
                        string url = repo.url.ToString();
                        await git.CloneRepo(url, targetdir, "HEAD", statusAction, spinnerAction);
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
        }

        #endregion

        #region actions
        private void SpinnerAction(string actionName, bool isRunning)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    if (isRunning)
                    {
                        actionLabel.Content = actionName;
                        loadingSpinner.Spin = true;
                        loadingSpinner.Visibility = Visibility.Visible;
                    }
                    else if (!string.IsNullOrWhiteSpace(actionName) && !isRunning)
                    {
                        actionLabel.Content = actionName;
                        loadingSpinner.Spin = false;
                        loadingSpinner.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        actionLabel.Content = "";
                        loadingSpinner.Spin = false;
                        loadingSpinner.Visibility = Visibility.Hidden;
                    }
                })
            );
        }
        private void ProgressBarAction(int completed, int total)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    if (ProgressBar.Maximum < total)
                        ProgressBar.Maximum = total;

                    ProgressBar.Value = completed;

                    if (completed == total)
                    {
                        MessageBox.Show(mainWindow, "Git operation complete.", "Operation finished", MessageBoxButton.OK);
                        ProgressBar.Value = 0;
                        ProgressBar.Maximum = 0;
                    }
                }
            )
            );
        }

        #endregion
    }
}
