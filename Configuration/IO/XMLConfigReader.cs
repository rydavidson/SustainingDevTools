using rydavidson.Accela.Configuration.ConfigModels;
using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace rydavidson.Accela.Configuration.IO
{
    class XMLConfigReader : IConfigReader
    {
        string PathToConfigFile { get; set; }
        bool isJetspeed { get; set; }

        const string XMLKEY_DRIVER = "driver";
        const string XMLKEY_DRIVER_CLASS = "driverclass";
        const string XMLKEY_CONNECTION_URL = "connectionurl";
        const string XMLKEY_EXCEPTION_SORTER_CLASS = "exceptionsorterclass";
        const string XMLKEY_SHARE_PREPARED_STATEMENTS = "sharepreparedstatements";
        const string XMLKEY_CHECK_VALID_CONNECTION_SQL = "checkvalidconnectionsql";

        private XmlDocument ConfigXMLData = new XmlDocument();
        private XmlNamespaceManager ConfigXMLNamespaceManager;

        public XMLConfigReader(string _pathToConfigFile, bool isJetspeed)
        {
            PathToConfigFile = _pathToConfigFile;
        }

        public IAccelaConfig Load()
        {
            LoadXML();

            if (isJetspeed)
                return LoadJetspeed();
            return LoadAA();
        }

        private void LoadXML()
        {
            using (XmlTextReader tr = new XmlTextReader(PathToConfigFile))
            {
                tr.Namespaces = true;
                ConfigXMLData.Load(tr);
            }
            ConfigXMLNamespaceManager = new XmlNamespaceManager(ConfigXMLData.NameTable);
            ConfigXMLNamespaceManager.AddNamespace("root", "urn:jboss:domain:1.4");
            ConfigXMLNamespaceManager.AddNamespace("datasources", "urn:jboss:domain:datasources:1.1");
        }

        public XmlNode GetAAValueByKey(string key)
        {
            XmlNode ConnectionUrlNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:connection-url", ConfigXMLNamespaceManager);
            XmlNode DriverNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:driver", ConfigXMLNamespaceManager);
            XmlNode DriverClassNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:driver-class", ConfigXMLNamespaceManager);
            XmlNode ExceptionSorterClassNodeAttribute = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:validation/datasources:expection-sorter/@class-name", ConfigXMLNamespaceManager);
            XmlNode CheckValidConnectionSQLNode = null;
            if (DriverNode.InnerText.ToLower() == "oracle")
            {
                CheckValidConnectionSQLNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:validation/datasources:check-valid-connection-sql", ConfigXMLNamespaceManager);
            }
            XmlNode SharePreparedStatementsNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/AA\"]/datasources:statement/datasources:share-parepared-statements", ConfigXMLNamespaceManager);

            switch (key.ToLower())
            {
                case XMLKEY_DRIVER:
                    return DriverNode;
                case XMLKEY_DRIVER_CLASS:
                    return DriverClassNode;
                case XMLKEY_CONNECTION_URL:
                    return ConnectionUrlNode;
                case XMLKEY_EXCEPTION_SORTER_CLASS:
                    return ExceptionSorterClassNodeAttribute;
                case XMLKEY_SHARE_PREPARED_STATEMENTS:
                    return SharePreparedStatementsNode;
                case XMLKEY_CHECK_VALID_CONNECTION_SQL:
                    return CheckValidConnectionSQLNode;
                default:
                    throw new Exception($"XML node not found for {key}");
            }
        }

        public XmlNode GetJetspeedValueByKey(string key)
        {
            XmlNode ConnectionUrlNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:connection-url", ConfigXMLNamespaceManager);
            XmlNode DriverNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:driver", ConfigXMLNamespaceManager);
            XmlNode DriverClassNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:driver-class", ConfigXMLNamespaceManager);
            XmlNode ExceptionSorterClassNodeAttribute = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:validation/datasources:expection-sorter/@class-name", ConfigXMLNamespaceManager);
            XmlNode CheckValidConnectionSQLNode = null;
            if (DriverNode.InnerText.ToLower() == "oracle")
            {
                CheckValidConnectionSQLNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:validation/datasources:check-valid-connection-sql", ConfigXMLNamespaceManager);
            }
            XmlNode SharePreparedStatementsNode = ConfigXMLData.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:statement/datasources:share-parepared-statements", ConfigXMLNamespaceManager);

            switch (key.ToLower())
            {
                case XMLKEY_DRIVER:
                    return DriverNode;
                case XMLKEY_DRIVER_CLASS:
                    return DriverClassNode;
                case XMLKEY_CONNECTION_URL:
                    return ConnectionUrlNode;
                case XMLKEY_EXCEPTION_SORTER_CLASS:
                    return ExceptionSorterClassNodeAttribute;
                case XMLKEY_SHARE_PREPARED_STATEMENTS:
                    return SharePreparedStatementsNode;
                case XMLKEY_CHECK_VALID_CONNECTION_SQL:
                    return CheckValidConnectionSQLNode;
                default:
                    throw new Exception($"XML node not found for {key}");
            }
        }

        private JBossConfig LoadAA()
        {
             //doc.Save(@"C:\Dev\Work\AccelaRepos\Automation\AA_9.3.7_HF\AA-ENV\av.biz\conf\standalone-full-test.xml");

            if (GetAAValueByKey(XMLKEY_DRIVER).InnerText.ToLower() == "oracle")
                return new JBossConfig("oracle")
                {
                    DriverName = GetAAValueByKey(XMLKEY_DRIVER).InnerText,
                    DriverClass = GetAAValueByKey(XMLKEY_DRIVER_CLASS).InnerText,
                    ExceptionSorterClass = GetAAValueByKey(XMLKEY_EXCEPTION_SORTER_CLASS).InnerText,
                    ConnectionUrl = GetAAValueByKey(XMLKEY_CONNECTION_URL).InnerText,
                    SharePreparedStatements = bool.Parse(GetAAValueByKey(XMLKEY_SHARE_PREPARED_STATEMENTS).InnerText),
                    CheckValidConnectionSQL = GetAAValueByKey(XMLKEY_CHECK_VALID_CONNECTION_SQL).InnerText
                };

            return new JBossConfig("mssql")
            {
                DriverName = GetAAValueByKey(XMLKEY_DRIVER).InnerText,
                DriverClass = GetAAValueByKey(XMLKEY_DRIVER_CLASS).InnerText,
                ExceptionSorterClass = GetAAValueByKey(XMLKEY_EXCEPTION_SORTER_CLASS).InnerText,
                ConnectionUrl = GetAAValueByKey(XMLKEY_CONNECTION_URL).InnerText,
                SharePreparedStatements = bool.Parse(GetAAValueByKey(XMLKEY_SHARE_PREPARED_STATEMENTS).InnerText)
            };
        }

        private JBossConfig LoadJetspeed()
        {

            if (GetJetspeedValueByKey(XMLKEY_DRIVER).InnerText.ToLower() == "oracle")
                return new JBossConfig("oracle")
                {
                    DriverName = GetJetspeedValueByKey(XMLKEY_DRIVER).InnerText,
                    DriverClass = GetJetspeedValueByKey(XMLKEY_DRIVER_CLASS).InnerText,
                    ExceptionSorterClass = GetJetspeedValueByKey(XMLKEY_EXCEPTION_SORTER_CLASS).InnerText,
                    ConnectionUrl = GetJetspeedValueByKey(XMLKEY_CONNECTION_URL).InnerText,
                    SharePreparedStatements = bool.Parse(GetJetspeedValueByKey(XMLKEY_SHARE_PREPARED_STATEMENTS).InnerText),
                    CheckValidConnectionSQL = GetJetspeedValueByKey(XMLKEY_CHECK_VALID_CONNECTION_SQL).InnerText
                };

            return new JBossConfig("mssql")
            {
                DriverName = GetJetspeedValueByKey(XMLKEY_DRIVER).InnerText,
                DriverClass = GetAAValueByKey(XMLKEY_DRIVER_CLASS).InnerText,
                ExceptionSorterClass = GetJetspeedValueByKey(XMLKEY_EXCEPTION_SORTER_CLASS).InnerText,
                ConnectionUrl = GetJetspeedValueByKey(XMLKEY_CONNECTION_URL).InnerText,
                SharePreparedStatements = bool.Parse(GetJetspeedValueByKey(XMLKEY_SHARE_PREPARED_STATEMENTS).InnerText)
            };
        }

        public string GetPathToConfigFile()
        {
            return PathToConfigFile;
        }
    }
}
