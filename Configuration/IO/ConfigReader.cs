using rydavidson.Accela.Configuration.ConfigModels;
using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace rydavidson.Accela.Configuration.IO
{
    public class ConfigReader : IConfigReader
    {
        private string pathToConfigFile;

        public ConfigReader(string _pathToConfigFile)
        {
            pathToConfigFile = _pathToConfigFile;

        }
        public string FindValue(string _key) // get a config value from a given key
        {
            string content = File.ReadAllText(pathToConfigFile, Encoding.Default);
            int indexOfKey = content.IndexOf(_key);
            if (indexOfKey == -1)
                return "";
            int indexEndOfValue = content.IndexOf(Environment.NewLine, indexOfKey);
            int indexOfEquals = content.IndexOf("=", indexOfKey);
            string value = content.Substring(indexOfEquals + 1, (indexEndOfValue - indexOfEquals) - 1);
            return value.ToLower();
        }

        public Dictionary<string, string> FindValues(List<string> _keys)
        {
            Dictionary<string, string> configPairs = new Dictionary<string, string>();
            foreach (string key in _keys)
            {
                configPairs.Add(key, FindValue(key));
            }
            return configPairs;
        }

        public IAccelaConfig Load()
        {
            AVServerConfig config = new AVServerConfig
            {
                DatabaseType = FindValue("av.db"),
                AvDbHost = FindValue("av.db.host"),
                AvDbName = FindValue("av.db.sid"),
                AvComponent = "av." + FindValue("av.server"),
                AvJetspeedDbName = FindValue("av.jetspeed.db.sid"),
                AvUser = FindValue("av.db.user"),
                AvJetspeedUser = FindValue("av.jetspeed.db.user"),
                Port = FindValue("av.db.port")
            };
            config.SetAvDatabasePassword(FindValue("av.db.password"));
            config.SetJetspeedDatabasePassword(FindValue("av.jetspeed.db.password"));

            if (config.DatabaseType == "mssql")
                return config as MssqlConfig;

            return config as OracleConfig;
        }
    }
}
