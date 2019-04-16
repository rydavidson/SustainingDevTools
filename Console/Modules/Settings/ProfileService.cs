using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Console.Common;

namespace Console.Modules.Settings
{
    class ProfileService
    {

        public string PathToProfileFile { get; set; }

        private List<ProfileModel> LoadedProfilesList { get; }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Cache cache = Cache.Instance;

        public ProfileService()
        {
            Init();
        }

        public ProfileService(string pathToProfileFile)
        {
            PathToProfileFile = pathToProfileFile;
            LoadedProfilesList = new List<ProfileModel>();
            Init();
        }

        private void Init()
        {
            InitLogger(cache.Settings["log_file"]);
        }

        private void InitLogger(string _logFile, bool areSettingsLoaded = true) //TODO: handle when settings aren't loaded
        {
            if (areSettingsLoaded)
            {
                _logFile = $"{cache.Settings["logging_dir"]}\\{_logFile}";

                var config = new NLog.Config.LoggingConfiguration();
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _logFile };
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
                config.AddRule(LogLevel.FromString(cache.Settings["logging_level"]), LogLevel.Fatal, logfile);

                LogManager.Configuration = config;
                LogManager.ReconfigExistingLoggers();
            }
        }

        private async Task LoadProfilesFromFile(string pathToFile)
        {
            string profileJSON = "{}";
            try
            {
                using (var stream = File.OpenRead(pathToFile))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        profileJSON = await reader.ReadToEndAsync().ConfigureAwait(continueOnCapturedContext: false);
                        if (string.IsNullOrWhiteSpace(profileJSON))
                            return; // Don't try to load an empty file
                        List<ProfileModel> profileData = JsonConvert.DeserializeObject<List<ProfileModel>>(profileJSON);
                    }
                }
            }
            catch (FileNotFoundException fileNotFound)
            {
                logger.Error(fileNotFound, "Unable to locate {file}", pathToFile);
            }
            catch (JsonSerializationException jsonErr)
            {
                logger.Error(jsonErr, "An error occured while deserializing {path}", PathToProfileFile);
                logger.Debug($"Failed profile data is {profileJSON}");
            }
            catch (Exception unknownError)
            {
                logger.Error(unknownError, "An unknown error occured");
            }
        }

        private async Task SaveProfilesToFile(string pathToFile)
        {
            try
            {
                if (!File.Exists(pathToFile))
                {
                    File.Create(pathToFile).Dispose();
                }

                byte[] encodedProfilesContent = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(LoadedProfilesList));

                using (FileStream sourceStream = new FileStream(pathToFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedProfilesContent, 0, encodedProfilesContent.Length).ConfigureAwait(continueOnCapturedContext: false);
                };
            }
            catch (FileNotFoundException fileNotFound)
            {
                logger.Error(fileNotFound.Message + fileNotFound.StackTrace);
            }
            catch (JsonSerializationException serializationError)
            {
                logger.Error(serializationError, "An error occured during serialization");
                logger.Error(serializationError.Message + serializationError.StackTrace);
            }
            catch (Exception unknownError)
            {
                logger.Error(unknownError, "An unknown error occured while saving profiles");
                logger.Error(unknownError.Message + unknownError.StackTrace);
            }
        }

        public List<ProfileModel> GetProfiles()
        {
            if (LoadedProfilesList.Count > 0)
                return LoadedProfilesList;
            LoadProfilesFromFile(PathToProfileFile).Wait();
            return LoadedProfilesList;
        }

        public ProfileModel GetProfileByName(string profileNameToFind)
        {
            if (LoadedProfilesList.Count > 0)
                return LoadedProfilesList.Find(profile => profile.Name.ToLowerInvariant() == profileNameToFind.ToLowerInvariant());
            LoadProfilesFromFile(PathToProfileFile).Wait();
            return LoadedProfilesList.Find(profile => profile.Name.ToLowerInvariant() == profileNameToFind.ToLowerInvariant());
        }

        public void AddProfileToList(ProfileModel profileToAdd)
        {
            LoadedProfilesList.Add(profileToAdd);
        }

        public void SaveProfiles()
        {
            SaveProfilesToFile(PathToProfileFile).Wait();
        }
    }
}
