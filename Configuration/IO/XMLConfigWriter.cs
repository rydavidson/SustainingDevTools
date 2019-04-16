using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace rydavidson.Accela.Configuration.IO
{
    class XMLConfigWriter : IConfigWriter
    {
        string PathToConfigFile;
        XmlDocument ConfigFile;

        public XMLConfigWriter(string _pathToConfigFile)
        {
            PathToConfigFile = _pathToConfigFile;

            ConfigFile = ReadXML(PathToConfigFile);
            XmlNamespaceManager manager = new XmlNamespaceManager(ConfigFile.NameTable);
            manager.AddNamespace("root", "urn:jboss:domain:1.4");
            manager.AddNamespace("datasources", "urn:jboss:domain:datasources:1.1");            

        }

        public void WriteValueToConfig(string key, string value)
        {
            
            switch (key.ToLower())
            {
                case "driver":
                    break;
                case "driverclass":
                    break;
                case "connectionurl":
                    break;
                case "exceptionsorterclass":
                    break;
                case "sharepreparedstatements":
                    break;
                case "checkvalidconnectionsql":
                    break;
            }
        }

        public string GetPathToConfigFile()
        {
            return PathToConfigFile;
        }

        private XmlDocument ReadXML(string path)
        {
            XmlDocument doc = new XmlDocument();
            using (XmlTextReader tr = new XmlTextReader(path))
            {
                tr.Namespaces = true;
                doc.Load(tr);
            }
            return doc;

        }
    }
}
