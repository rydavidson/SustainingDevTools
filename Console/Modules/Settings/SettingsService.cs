using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NLog;
using Console.Common;

namespace Console.Modules.Settings
{
    public class SettingsService
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string DefaultSettingsFile = @"Resources\Settings.json";
        private const string DefautLogDirectory = @"Logs\";
        private const string DefaultLogFile = "Console.log";
        private readonly LogLevel DefaultFileLoggingLevel = LogLevel.Info;
        private readonly Cache cache = Cache.Instance;
        

        private string pathToContentFile;

        private dynamic contentData;
        private dynamic settingsData;

        private Dictionary<string, string> settings;

        #region constructors

        public SettingsService(string _pathToSettingsFile = DefaultSettingsFile)
        {
            settings = new Dictionary<string, string>();

            Init(_pathToSettingsFile);
        }
        #endregion

        #region public methods
        public async Task ReloadSettings(bool clearCache = false)
        {
            if (clearCache)
                cache.Settings = null;

            await LoadSettingsFromFile();
        }

        #endregion

        #region private methods

        #region init

        private void Init(string _pathToSettingsFile)
        {

            InitLogger(DefaultLogFile,false);
            Task loadSettingsTask = LoadSettingsFromFile(_pathToSettingsFile);
            loadSettingsTask.Wait();

            cache.LoggingLevel = LogLevel.FromString(settings["logging_level"]);
            InitLogger(settings["log_file"]);

            DownloadContent(false);

            Task loadContentTask = LoadContentFromFile(pathToContentFile);
            loadContentTask.Wait();
        }
        private void InitLogger(string _logFile = DefaultLogFile, bool areSettingsLoaded = true)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _logFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);

            // If the settings are loaded, get the log directory from settings, otherwise use the default. Prepend the directory to the logfile path
            _logFile += areSettingsLoaded ? $"{settings["logging_dir"]}\\" : DefautLogDirectory;   
            // If settings are loaded, get the log level from settings, otherwise set the log level to Info
            config.AddRule(areSettingsLoaded ? LogLevel.FromString(settings["logging_level"]) : DefaultFileLoggingLevel, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        #endregion

        #region IO

        private async Task LoadContentFromFile(string _pathToContentFile)
        {
            if (cache.isContentLoaded)
            {
                logger.Debug("Using cached content");
                return;
            }

            logger.Debug($"Loading content from {_pathToContentFile}");

            try
            {
                using (var stream = File.OpenRead(_pathToContentFile))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string data = await reader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);
                        contentData = JsonConvert.DeserializeObject(data);
                        //logger.Debug(contentData.repositories[0]);
                    }
                }

                cache.Content = contentData;
                logger.Info($"Loaded content from {_pathToContentFile}");

            }
            catch (FileNotFoundException fe)
            {
                logger.Error($"Failed to load content. File not found: {_pathToContentFile}. \n {fe.StackTrace}");
            }
            catch (Exception e)
            {
                logger.Error($"An unknown error occured while loading content from {_pathToContentFile}. \n {e.StackTrace}");
                logger.Error(e.Message, e.StackTrace);
            }
        }

        private void DownloadContent(bool doOverwrite)
        {

            pathToContentFile = $"{Path.GetFullPath(settings["resources_dir"])}\\{settings["content_file"]}";

            if (!File.Exists(pathToContentFile) || doOverwrite)
            {
                try
                {
                    logger.Info($"Downloading content file from {settings["content_url"]}");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(new System.Uri(settings["content_url"]), pathToContentFile);
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"Failed to download content file from {settings["content_url"]}. \n {e.Message}{e.StackTrace}");
                }

            }
        }

        private async Task LoadSettingsFromFile(string _pathToSettingsFile = DefaultSettingsFile)
        {
            if (cache.Settings != null)
            {
                logger.Debug("Using cached settings");
            }

            logger.Debug($"Loading settings from {_pathToSettingsFile}");

            try
            {
                using (var stream = File.OpenRead(_pathToSettingsFile))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string data = await reader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);
                        settingsData = JsonConvert.DeserializeObject(data);
                    }
                }


                MergeSettings();

                cache.Settings = settings;
                logger.Info($"Loaded settings from {_pathToSettingsFile}");

                pathToContentFile = settings["resources_dir"] + "\\" + settings["content_file"];

                logger.Debug($"Path to content file: {pathToContentFile}");
            }
            catch (FileNotFoundException fe)
            {
                logger.Error($"Failed to load settings. File not found: {_pathToSettingsFile}. \n {fe.StackTrace}");
            }
            //catch(KeyNotFoundException ke)
            //{
            //    logger.Error($"Key not found");
            //}
            catch (Exception e)
            {
                logger.Error($"An unknown error occured while loading settings from {_pathToSettingsFile}. \n {e.StackTrace}");
                logger.Error(e.Message, e.StackTrace);
            }
        }

        public async Task WriteSettingsToFile(string file = DefaultSettingsFile)
        {

            foreach(var p in settingsData.user_settings)
            {
                PropertyInfo prop = settingsData.user_settings.GetType().GetProprty(p.name);

                if (settings.ContainsKey(prop.Name))
                {
                    prop.SetValue(settingsData.user_settings, settings[prop.Name]);
                }
            }

            //settingsData.user_settings.auto_check_for_updates = cache.Settings["auto_check_for_updates"];
            //settingsData.user_settings.show_debug_console = cache.Settings["show_debug_console"];
            //settingsData.user_settings.git_username = cache.Settings["git_username"];
            //settingsData.user_settings.git_password = cache.Settings["git_password"];
            //settingsData.user_settings.repo_base_dir = cache.Settings["repo_base_dir"];

            settingsData.last_updated = DateTime.Now.ToString();

            string jsonContent = JsonConvert.SerializeObject(settingsData);
            byte[] encodedContent = Encoding.Unicode.GetBytes(jsonContent);

            using (FileStream sourceStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedContent, 0, encodedContent.Length);
            };
        }

        #endregion


        #region business


        private void InitializeCache()
        {
            //cache.GitUsername = settings["git_username"];
            //cache.GitPassword = settings["git_password"];
        }

        public void UpdateGitCredentials(string username, string password)
        {
            settings["git_username"] = username;
            settings["git_password"] = password;
            InitializeCache();
        }

        private Dictionary<string, string> LoadAppSettings()
        {

            Dictionary<string, string> appSettings = ((IEnumerable<KeyValuePair<string, JToken>>)settingsData["app_settings"])
                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            return appSettings;
        }

        private Dictionary<string, string> LoadUserSettings()
        {
            Dictionary<string, string> userSettings = ((IEnumerable<KeyValuePair<string, JToken>>)settingsData["user_settings"])
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            return userSettings;

        }

        private void MergeSettings()
        {
            // iterate over the values and add them to the settings 
            LoadAppSettings().ToList().ForEach(x => settings.Add(x.Key, x.Value));
            LoadUserSettings().ToList().ForEach(x => settings.Add(x.Key, x.Value));
        }

        #endregion

        #region misc

        private void ReconfigLoggers()
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(cache.LoggingLevel);
            }

            LogManager.ReconfigExistingLoggers();
        }

        #endregion


        #endregion
    }
}
