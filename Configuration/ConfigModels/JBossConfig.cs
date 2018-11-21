using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rydavidson.Accela.Configuration.ConfigModels
{
    public sealed class JBossConfig : IAccelaConfig
    {
        public string DriverName { get; set; }
        public string DriverClass { get; set; }
        public string CheckValidConnectionSQL { get; set; }
        public string ExceptionSorterClass { get; set; }
        public string ConnectionUrl { get; set; }
        public bool SharePreparedStatements { get; set; }
        

        #region oracle constants

        private const string OracleDriverName = "oracle";
        private const string OracleDriverClass = "oracle.jdbc.driver.OracleDriver";
        private const string OracleCheckValidConnectionSQL = "select 1 from dual";
        private const string OracleConnectionUrl = @"jdbc:oracle:thin:@(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=${av.db.host})(PORT=${av.db.port})))(CONNECT_DATA=(SERVICE_NAME=${av.db.servicename})(SERVER=DEDICATED)))";
        private const string OracleExceptionSorterClass = "org.jboss.jca.adapters.jdbc.extensions.oracle.OracleExceptionSorter";
        private const bool OracleSharePreparedStatements = true;

        #endregion

        #region mssql constants

        private const string MSSQLDriverName = "accela";
        private const string MSSQLDriverClass = "com.vembu.jdbc.AVVembuDriver";
        private const string MSSQLConnectionUrl = @"${av.db.connection.prefix}${av.jetspeed.db.host}:${av.jetspeed.db.port};DatabaseName=${av.jetspeed.db.servicename}";
        private const string MSSQLExceptionSorterClass = "org.jboss.jca.adapters.jdbc.extensions.mssql.SqlServerExceptionSorter";
        private const bool MSSQLSharePreparedStatements = false;


        #endregion

        public JBossConfig(AVServerConfig avconfig)
        {
            DriverName = OracleDriverName;
            DriverClass = OracleDriverClass;
            CheckValidConnectionSQL = OracleCheckValidConnectionSQL;
            ExceptionSorterClass = OracleExceptionSorterClass;
            ConnectionUrl = OracleConnectionUrl;
            SharePreparedStatements = OracleSharePreparedStatements;

            if(avconfig.DatabaseType == "mssql")
            {
                DriverName = MSSQLDriverName;
                DriverClass = MSSQLDriverClass;
                CheckValidConnectionSQL = null;
                ExceptionSorterClass = MSSQLExceptionSorterClass;
                ConnectionUrl = MSSQLConnectionUrl;
                SharePreparedStatements = MSSQLSharePreparedStatements;
            }
        }

    }
}
