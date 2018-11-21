using System.Security;


namespace rydavidson.Accela.Configuration.ConfigModels
{
    public sealed class MssqlConfig : AVServerConfig
    {
        public MssqlConfig() { }

        public MssqlConfig(string _serverHostName, string _avDatabaseName, string _jetspeedDatabaseName) :
            this(_serverHostName, _avDatabaseName, _jetspeedDatabaseName, null, null, null, null)
        { }

        public MssqlConfig(string _serverHostName, string _avDatabaseName, string _jetspeedDatabaseName, string _avDatabaseUser, string _jetspeedDatabaseUser,
            SecureString _avDatabasePassword, SecureString _jetspeedDatabasePassword)
        {
            DatabaseType = "mssql";

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
