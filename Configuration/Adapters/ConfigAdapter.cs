using System;
using System.IO;
using rydavidson.Accela.Configuration.Common;
using rydavidson.Accela.Configuration.IO;
using rydavidson.Accela.Configuration.ConfigModels;
using rydavidson.Accela.Configuration.IO.Interfaces;

namespace rydavidson.Accela.Configuration.Adapters
{
    public class ConfigAdapter
    {
        public string PathToConfigFile { get; set; }
        public Logger Log { get; set; }

        // provide external messaging

        public Action<string> MessageHandler;
        public Action<string> ErrorMessageHandler;

        // private members
        private enum MODE { dir, file };

        private bool isMessageHandlerRegistered = false;
        private bool isErrorMessageHandlerRegistered = false;

        private IConfigWriter configWriter;
        private IConfigReader configReader;

        private MODE currentMode;


        #region constructors

        public ConfigAdapter(string _pathToConfigFile, bool isDirectory)
        {
            currentMode = MODE.file;
            if (isDirectory)
                currentMode = MODE.dir;

            //PathToConfigFile = _pathToConfigFile.Replace("\"","");
            PathToConfigFile = _pathToConfigFile;

            if (currentMode == MODE.file)
            {
                if (PathToConfigFile.Contains(".properties"))
                {
                    configWriter = new ConfigWriter(PathToConfigFile);
                    configReader = new ConfigReader(PathToConfigFile);
                }
                if (PathToConfigFile.Contains(".xml"))
                {
                    configWriter = new XMLConfigWriter(PathToConfigFile);
                    configReader = new XMLConfigReader(PathToConfigFile);
                }

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

        public MssqlConfig ReadConfigFromFile()
        {
            return null;
        }

        //public OracleConfig ReadConfigFromFile()
        //{

        //}

        #endregion

        #region writers

        public void WriteConfigToFile(MssqlConfig _mssql)
        {
            if (!File.Exists(PathToConfigFile))
            {
                SendError("File not found: " + PathToConfigFile);
                return;
            }
            if (!PathToConfigFile.Contains(".properties"))
            {
                SendError("Incorrect file type. Provide a .properties file");
                return;
            }

            if (File.Exists(PathToConfigFile + ".backup"))
                File.Delete(PathToConfigFile + ".backup");

            File.Copy(PathToConfigFile, PathToConfigFile + ".backup");

            configWriter.WriteValueToConfig("aa.database", _mssql.DatabaseType);
            configWriter.WriteValueToConfig("av.db", _mssql.DatabaseType);

            configWriter.WriteValueToConfig("av.db.host", _mssql.AvDbHost);
            configWriter.WriteValueToConfig("av.jetspeed.db.host", _mssql.AvDbHost);

            configWriter.WriteValueToConfig("av.db.sid", _mssql.AvDbName);
            configWriter.WriteValueToConfig("av.db.username", _mssql.AvUser);
            configWriter.WriteValueToConfig("av.db.password", _mssql.GetAvDatabasePassword());

            configWriter.WriteValueToConfig("av.jetspeed.db.sid", _mssql.AvJetspeedDbName);
            configWriter.WriteValueToConfig("av.jetspeed.db.username", _mssql.AvJetspeedUser);
            configWriter.WriteValueToConfig("av.jetspeed.db.password", _mssql.GetJetspeedDatabasePassword());

            SendMessage("Updated config successfully");
        }

        #endregion

        #region getters and setters

        public void SetConfigFile(string file, bool isRelativeToDir)
        {
            bool set = false;
            if (isRelativeToDir)
            {
                if (File.Exists(PathToConfigFile + file))
                    PathToConfigFile += file;
            }
            else
            {
                if (File.Exists(file))
                    PathToConfigFile = file;
            }
            if (!set)
            {
                SendError($"Unable to set config file to {file}");
            }
            else
            {
                currentMode = MODE.file;
            }
        }
        #endregion









    }
}
