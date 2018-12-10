using System;
using System.IO;
using rydavidson.Accela.Configuration.Common;
using rydavidson.Accela.Configuration.ConfigModels;
using rydavidson.Accela.Configuration.IO;
using rydavidson.Accela.Configuration.IO.Interfaces;

namespace rydavidson.Accela.Configuration.Adapters
{
    public class ConfigAdapter
    {
        private string PathToConfigFile { get; set; }
        public Logger Log { get; set; }

        // provide external messaging

        public Action<string> MessageHandler;
        public Action<string> ErrorMessageHandler;

        // private members
        private enum MODE { dir, file };
        private enum TYPE { properties, xml }

        private bool isMessageHandlerRegistered = false;
        private bool isErrorMessageHandlerRegistered = false;

        private IConfigWriter configWriter;
        private IConfigReader configReader;

        private MODE currentMode;
        private TYPE currentType;


        #region constructors
        public ConfigAdapter(string _pathToConfigFile, bool isDirectory)
        {
            currentMode = MODE.file;
            if (isDirectory)
                currentMode = MODE.dir;

            PathToConfigFile = _pathToConfigFile;

            if (currentMode == MODE.file)
            {
                if (PathToConfigFile.Contains(".properties"))
                {
                    currentType = TYPE.properties;
                    configWriter = new ConfigWriter(PathToConfigFile);
                    configReader = new ConfigReader(PathToConfigFile);
                }
                if (PathToConfigFile.Contains(".xml"))
                {
                    currentType = TYPE.xml;
                    configWriter = new XMLConfigWriter(PathToConfigFile);
                    configReader = new XMLConfigReader(PathToConfigFile);
                }
            }
            if (currentMode == MODE.dir)
            {
                // Do properties by default, override it later
                BuildIO(TYPE.properties, PathToConfigFile + "\\conf\\av\\ServerConfig.properties");
            }
        }

        private void BuildIO(TYPE type, string pathToConfigFile)
        {
            if (type == TYPE.properties)
            {
                configWriter = new ConfigWriter(pathToConfigFile);
                configReader = new ConfigReader(pathToConfigFile);
            }
            else
            {
                configWriter = new XMLConfigWriter(pathToConfigFile);
                configReader = new XMLConfigReader(pathToConfigFile);
            }

        }
        #endregion



        #region register delegates

        public void RegisterMessageDelegate(Action<string> _messageHandler)
        {
            MessageHandler = _messageHandler;
            isMessageHandlerRegistered = true;
        }

        public void RegisterErrorMessageDelegate(Action<string> _errorMessageHander)
        {
            ErrorMessageHandler = _errorMessageHander;
            isErrorMessageHandlerRegistered = true;
        }

        #endregion

        #region message senders

        private void SendMessage(string _message)
        {
            if (isMessageHandlerRegistered)
                MessageHandler(_message);
        }

        private void SendError(string _err)
        {
            if (isErrorMessageHandlerRegistered)
                ErrorMessageHandler(_err);
        }

        #endregion

        #region readers

        public IAccelaConfig ReadConfigFromFile()
        {
            return configReader.Load();
        }

        #endregion

        #region writers

        public void WriteConfigToFile(IAccelaConfig _config)
        {
            if (_config.GetType() == typeof(JBossConfig))
            {
                currentType = TYPE.xml;
                BuildIO(TYPE.xml, PathToConfigFile + "conf\\standalone-full.xml");
            }
            if (!File.Exists(configWriter.GetPathToConfigFile()))
            {
                SendError("File not found: " + PathToConfigFile);
                return;
            }

            if (File.Exists(configWriter.GetPathToConfigFile() + ".backup"))
                File.Delete(configWriter.GetPathToConfigFile() + ".backup");
            File.Copy(configWriter.GetPathToConfigFile(), configWriter.GetPathToConfigFile() + ".backup");

            if (currentType == TYPE.properties)
            {
                switch ((_config as AVServerConfig).DatabaseType.ToLower())
                {
                    case "mssql":
                        WriteAVConfig((MssqlConfig)_config);
                        break;
                    case "oracle":
                        WriteAVConfig((OracleConfig)_config);
                        break;
                }
            }
            else
            {
                WriteJbossConfig((JBossConfig)_config);
            }

            // End of setting values
            SendMessage("Updated config successfully");
        }

        private void WriteAVConfig(AVServerConfig _config)
        {
            configWriter.WriteValueToConfig("av.db", _config.DatabaseType.ToLower());
            configWriter.WriteValueToConfig("aa.database", _config.DatabaseType.ToLower());
            configWriter.WriteValueToConfig("av.db.host", _config.AvDbHost);
            // Handle web specific differences
            if (PathToConfigFile.Contains("av.web"))
            {
                configWriter.WriteValueToConfig("av.db.servicename", _config.AvJetspeedDbName);
                configWriter.WriteValueToConfig("av.db.sid", _config.AvJetspeedDbName);
                configWriter.WriteValueToConfig("av.db.username", _config.AvJetspeedUser);
                configWriter.WriteValueToConfig("av.db.password", _config.GetJetspeedDatabasePassword());
            }
            else
            {
                configWriter.WriteValueToConfig("av.db.sid", _config.AvDbName);
                configWriter.WriteValueToConfig("av.db.servicename", _config.AvDbName);
                configWriter.WriteValueToConfig("av.db.username", _config.AvUser);
                configWriter.WriteValueToConfig("av.db.password", _config.GetAvDatabasePassword());
            }
            // Handle biz specific differences
            if (PathToConfigFile.Contains("av.biz"))
            {
                configWriter.WriteValueToConfig("av.jetspeed.db.host", _config.AvDbHost);
                configWriter.WriteValueToConfig("av.jetspeed.db.sid", _config.AvJetspeedDbName);
                configWriter.WriteValueToConfig("av.jetspeed.db.servicename", _config.AvJetspeedDbName);
                configWriter.WriteValueToConfig("av.jetspeed.db.username", _config.AvJetspeedUser);
                configWriter.WriteValueToConfig("av.jetspeed.db.password", _config.GetJetspeedDatabasePassword());
            }

            switch (_config.DatabaseType.ToLower())
            {
                case "mssql":
                    WriteMSSQLSpecificInfo();
                    break;
                case "oracle":
                    WriteOracleSpecificInfo();
                    break;
            }
        }

        private void WriteOracleSpecificInfo()
        {
            configWriter.WriteValueToConfig("av.db.driver.real.connection.prefix", "@av.db.driver.real.connection.prefix@");
            configWriter.WriteValueToConfig("av.db.connection.prefix", "jdbc:oracle:thin:@");
            configWriter.WriteValueToConfig("av.db.connection.sid.prefix", ":");
            configWriter.WriteValueToConfig("av.db.driver.class", "oracle.jdbc.driver.OracleDriver");
            configWriter.WriteValueToConfig("av.db.connection.suffix", ";sendStringParametersAsUnicode=false");
            configWriter.WriteValueToConfig("encrypted.av.db.driver.password", "8637E4258556EC0FED924EB6AC4BE4CE0E29B7A1A17FBDAB");
        }

        private void WriteMSSQLSpecificInfo()
        {
            configWriter.WriteValueToConfig("av.db.driver.real.connection.prefix", "jdbc:sqlserver://");
            configWriter.WriteValueToConfig("av.db.connection.prefix", "jdbc:accela:sqlserver://");
            configWriter.WriteValueToConfig("av.db.driver.class", "com.microsoft.sqlserver.jdbc.SQLServerDriver");
            configWriter.WriteValueToConfig("av.db.connection.suffix", ";sendStringParametersAsUnicode=false");
            configWriter.WriteValueToConfig("av.db.driver.password", "Vantage360Rules!");
        }

        private void WriteJbossConfig(JBossConfig _config)
        {
           
        }

        #endregion

        #region getters and setters

        //public void SetConfigFile(string file, bool isRelativeToDir)
        //{
        //    bool set = false;
        //    if (isRelativeToDir)
        //    {
        //        if (File.Exists(PathToConfigFile + file))
        //            PathToConfigFile += file;
        //    }
        //    else
        //    {
        //        if (File.Exists(file))
        //            PathToConfigFile = file;
        //    }
        //    if (!set)
        //    {
        //        SendError($"Unable to set config file to {file}");
        //    }
        //    else
        //    {
        //        currentMode = MODE.file;
        //    }
        //}
        #endregion









    }
}
