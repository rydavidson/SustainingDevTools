using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rydavidson.Accela.Configuration.Adapters;
using rydavidson.Accela.Configuration.ConfigModels;
using System.IO;
using Console.Common;
using System.Diagnostics;

namespace Console.Modules.Configuration
{
    /// <summary>
    /// Provides an abstract interface to perform common configuration related functions
    /// </summary>
    public class ConfigFacade
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Cache cache = Cache.Instance;
        private Dictionary<string, DirectoryInfo> compDirs;
        private Action<string> infoAction = (string message) => logger.Info(message);
        private Action<string> errorAction = (string err) => logger.Error(err);
        private string[] supportedComponents = new string[] { "av.biz", "av.cfmx_railo", "av.indexer", "av.web" };
        
        /// <summary>
        /// Construct a ConfigFacade
        /// </summary>
        /// <param name="_baseDir">
        /// The root directory for the AA environment. 
        /// The root directory should contain the component subdirectories (i.e. av.biz) directory below. 
        /// </param>
        public ConfigFacade(DirectoryInfo _baseDir)
        {
            if (_baseDir.Exists)
                cache.EnvBaseDir = _baseDir;

            compDirs = new Dictionary<string, DirectoryInfo>();

            foreach (DirectoryInfo dir in _baseDir.EnumerateDirectories())
            {
                if (dir.FullName.Contains("av."))
                {
                    string cDir = dir.FullName.Substring(dir.FullName.LastIndexOf('\\'), dir.FullName.Length - (dir.FullName.LastIndexOf('\\') + 1));
                    cDir.Trim();
                    
                    if (cDir.Contains("\\"))
                    {
                        logger.Debug($"The substring for the component sub directory {dir.FullName} got messed up. Removing the backslash");
                        cDir.Replace("\\","");
                        logger.Debug($"Current component sub directory value: {cDir}");
                    }

                    if (Directory.Exists($"{dir.FullName}\\conf\\av") && supportedComponents.Contains(cDir))
                    {
                        compDirs.Add(cDir, dir);                        
                        cache.ConfigAdapters.Add(cDir, new ConfigAdapter(dir.FullName, true));
                        cache.ConfigAdapters[cDir].RegisterMessageDelegate(infoAction);
                        cache.ConfigAdapters[cDir].RegisterErrorMessageDelegate(errorAction);
                    }
                }
            }  
        }

        //private List<string> GetConfigFilesForComponent(string componentDirectory)
        //{

        //}

        public void SetDatabaseConfig(AVServerConfig _conf)
        {

        }

        public AVServerConfig GetDatabaseConfig()
        {
            return new AVServerConfig();
        }
    }
}
