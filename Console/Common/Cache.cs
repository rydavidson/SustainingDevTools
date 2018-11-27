using rydavidson.Accela.Configuration.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Security;
using System.Diagnostics;

namespace Console.Common
{
    public class Cache
    {
        private static Cache instance = null;
        private static readonly object O = new object();


        #region backing fields
        private Dictionary<string, ConfigAdapter> _configAdapters;
        private Dictionary<string, string> _settings;
        private bool _isContentLoaded;
        private dynamic _content;


        #endregion

        #region properties

        public DirectoryInfo EnvBaseDir { get; set; }
        public Dictionary<string, string> Settings
        {
            get { return _settings; }
            set
            {
                areSettingsLoaded = true;
                _settings = value;
            }
        }
        public dynamic Content {
            get { return _content; }
            set
            {
                isContentLoaded = true;
                _content = value;
            }
        }
        public Dictionary<string, ConfigAdapter> ConfigAdapters
        {
            get
            {
                if (_configAdapters is null)
                    _configAdapters = new Dictionary<string, ConfigAdapter>();
                return _configAdapters;
            }
            set
            { _configAdapters = value; }
        }
        public bool areSettingsLoaded { get; set; }
        public bool isContentLoaded {
            get; set;
        }

        public enum Components { biz, cfmx_railo, indexer, web }

        public LogLevel LoggingLevel { get; set; }

        public static Cache Instance
        {
            get
            {
                lock (O)
                {
                    if (instance == null)
                    {
                        instance = new Cache();
                    }
                    return instance;
                }
            }
        }

        #endregion

        Cache() { }


    }
}
