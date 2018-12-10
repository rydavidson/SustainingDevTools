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
        public XMLConfigReader(string _pathToConfigFile, bool isJetspeed)
        {
            PathToConfigFile = _pathToConfigFile;
        }

        public IAccelaConfig Load()
        {
            if (isJetspeed)
                return LoadJetspeed();
             return LoadAA();
        }

        private JBossConfig LoadAA()
        {
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


            doc.Save(@"C:\Dev\Work\AccelaRepos\Automation\AA_9.3.7_HF\AA-ENV\av.biz\conf\standalone-full-test.xml");

            if (DriverNode.InnerText.ToLower() == "oracle")
                return new JBossConfig("oracle")
                {
                    DriverName = DriverNode.InnerText,
                    DriverClass = DriverClassNode.InnerText,
                    ExceptionSorterClass = ExceptionSorterClassNodeAttribute.InnerText,
                    ConnectionUrl = ConnectionUrlNode.InnerText,
                    SharePreparedStatements = bool.Parse(SharePreparedStatementsNode.InnerText),
                    CheckValidConnectionSQL = CheckValidConnectionSQLNode.InnerText
                };

            return new JBossConfig("mssql")
            {
                DriverName = DriverNode.InnerText,
                DriverClass = DriverClassNode.InnerText,
                ExceptionSorterClass = ExceptionSorterClassNodeAttribute.InnerText,
                ConnectionUrl = ConnectionUrlNode.InnerText,
                SharePreparedStatements = bool.Parse(SharePreparedStatementsNode.InnerText)
            };
        }

        private JBossConfig LoadJetspeed()
        {
            XmlDocument doc = new XmlDocument();
            using (XmlTextReader tr = new XmlTextReader(PathToConfigFile))
            {
                tr.Namespaces = true;
                doc.Load(tr);
            }
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("root", "urn:jboss:domain:1.4");
            manager.AddNamespace("datasources", "urn:jboss:domain:datasources:1.1");

            XmlNode ConnectionUrlNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:connection-url", manager);
            XmlNode DriverNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:driver", manager);
            XmlNode DriverClassNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:driver-class", manager);
            XmlNode ExceptionSorterClassNodeAttribute = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:validation/datasources:expection-sorter/@class-name", manager);
            XmlNode CheckValidConnectionSQLNode = null;
            if (DriverNode.InnerText.ToLower() == "oracle")
            {
                CheckValidConnectionSQLNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:validation/datasources:check-valid-connection-sql", manager);
            }
            XmlNode SharePreparedStatementsNode = doc.SelectSingleNode("/root:server/root:profile/datasources:subsystem/datasources:datasources/datasources:datasource[@jndi-name=\"java:/JetspeedDS\"]/datasources:statement/datasources:share-parepared-statements", manager);


            doc.Save(@"C:\Dev\Work\AccelaRepos\Automation\AA_9.3.7_HF\AA-ENV\av.biz\conf\standalone-full-test.xml");

            if (DriverNode.InnerText.ToLower() == "oracle")
                return new JBossConfig("oracle")
                {
                    DriverName = DriverNode.InnerText,
                    DriverClass = DriverClassNode.InnerText,
                    ExceptionSorterClass = ExceptionSorterClassNodeAttribute.InnerText,
                    ConnectionUrl = ConnectionUrlNode.InnerText,
                    SharePreparedStatements = bool.Parse(SharePreparedStatementsNode.InnerText),
                    CheckValidConnectionSQL = CheckValidConnectionSQLNode.InnerText
                };

            return new JBossConfig("mssql")
            {
                DriverName = DriverNode.InnerText,
                DriverClass = DriverClassNode.InnerText,
                ExceptionSorterClass = ExceptionSorterClassNodeAttribute.InnerText,
                ConnectionUrl = ConnectionUrlNode.InnerText,
                SharePreparedStatements = bool.Parse(SharePreparedStatementsNode.InnerText)
            };
        }

        public string GetPathToConfigFile()
        {
            return PathToConfigFile;
        }
    }
}
