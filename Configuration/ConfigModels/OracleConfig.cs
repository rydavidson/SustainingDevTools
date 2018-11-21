using System.Security;


namespace rydavidson.Accela.Configuration.ConfigModels
{
    public sealed class OracleConfig : AVServerConfig
    {
        public OracleConfig() { }

        public OracleConfig(string _serverHostName, string _avDatabaseName, string _jetspeedDatabaseName) :
            this(_serverHostName, _avDatabaseName, _jetspeedDatabaseName, null, null, null, null)
        { }

        public OracleConfig(string _serverHostName, string _avDatabaseName, string _jetspeedDatabaseName, string _avDatabaseUser, string _jetspeedDatabaseUser,
            SecureString _avDatabasePassword, SecureString _jetspeedDatabasePassword)
        {
            DatabaseType = "oracle";

            AvDbHost = _serverHostName;
            AvDbName = _avDatabaseName;
            AvJetspeedDbName = _jetspeedDatabaseName;
            AvUser = _avDatabaseUser;
            AvJetspeedUser = _jetspeedDatabaseUser;
            SetAvDatabasePassword(_avDatabasePassword);
            SetJetspeedDatabasePassword(_jetspeedDatabasePassword);
        }
    }
}
