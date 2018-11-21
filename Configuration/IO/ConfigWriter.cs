using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rydavidson.Accela.Configuration.IO
{
    class ConfigWriter :  IConfigWriter
    {
        private string pathToConfigFile;

        public ConfigWriter(string _path)
        {
            pathToConfigFile = _path;
        }

        public void WriteValueToConfig(string _key, string _val)
        {
            if ((_key == "av.db.host" || _key == "av.jetspeed.db.host") && _val.Contains(":"))
                _val = StripPortFromHost(_key, _val);

            string content = File.ReadAllText(pathToConfigFile, Encoding.Default); // read in the config file
            int indexOfKey = content.IndexOf(_key); // get the start of the config item
            if (indexOfKey == -1)
                return; // exit if the config item isn't found

            int indexEndOfLine = content.IndexOf(Environment.NewLine, indexOfKey); // get the end of the config item
            int indexOfEquals = content.IndexOf("=", indexOfKey); // get the index of the equals sign after the config item
            string oldValue = indexOfEquals == -1 ? content.Substring(indexOfKey, indexEndOfLine - indexOfKey) : content.Substring(indexOfEquals + 1, (indexEndOfLine - indexOfEquals)).Trim();
            string oldConfigLine = content.Substring(indexOfKey, (indexEndOfLine - indexOfKey)); // get the entire line
            string newConfigLine = "";
            if (oldValue == "" || oldValue == null) // handle empty values
            {
                if (oldConfigLine.Contains("="))
                    newConfigLine = oldConfigLine + _val; 
                if (!oldConfigLine.Contains("="))
                    newConfigLine = oldConfigLine + "=" + _val;
            }
            else
            {
                newConfigLine = oldConfigLine.Replace(oldValue, _val); // replace the old value in the line with the new value
            }

            string newFile = content.Replace(oldConfigLine, newConfigLine);

            File.WriteAllText(pathToConfigFile, newFile, Encoding.Default);

        }

        private string StripPortFromHost(string _key, string _hostWithPort)
        {
            string port = _hostWithPort.Substring(_hostWithPort.IndexOf(":") + 1, _hostWithPort.Length - _hostWithPort.IndexOf(":") - 1);
            switch (_key)
            {
                case "av.db.host":
                    new ConfigWriter(pathToConfigFile).WriteValueToConfig("av.db.port", port);
                    break;
                case "av.jetspeed.db.host":
                    new ConfigWriter(pathToConfigFile).WriteValueToConfig("av.jetspeed.db.port", port);
                    break;
                default:
                    break;
            }
            return _hostWithPort.Remove(_hostWithPort.IndexOf(":"));
        }
    }
}
