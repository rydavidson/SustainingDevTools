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

        public XMLConfigWriter(string _pathToConfigFile)
        {
            PathToConfigFile = _pathToConfigFile;

            XmlDocument doc = new XmlDocument();
            using (XmlTextReader tr = new XmlTextReader(PathToConfigFile))
            {
                tr.Namespaces = true;
                doc.Load(tr);
            }
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("root", "urn:jboss:domain:1.4");
            manager.AddNamespace("datasources", "urn:jboss:domain:datasources:1.1");

            XmlNode ConnectionUrlNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:connection-url", manager);
            XmlNode DriverNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:driver", manager);
            XmlNode DriverClassNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:driver-class", manager);
            XmlNode ExceptionSorterClassNodeAttribute = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:validation/datasources:expection-sorter/@class-name", manager);
            XmlNode CheckValidConnectionSQLNode = null;
            if (DriverNode.InnerText.ToLower() == "oracle")
            {
                CheckValidConnectionSQLNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:validation/datasources:check-valid-connection-sql", manager);
            }
            XmlNode SharePreparedStatementsNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:statement/datasources:share-parepared-statements", manager);
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
    }
}
