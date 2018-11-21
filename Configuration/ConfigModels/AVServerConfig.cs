using rydavidson.Accela.Configuration.Common;
using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace rydavidson.Accela.Configuration.ConfigModels
{
    public class AVServerConfig : IAccelaConfig
    {
        public string DatabaseType { get; set; }
        public string AvDbHost { get; set; }
        public string AvDbName { get; set; }
        public string AvJetspeedDbName { get; set; }
        public string AvUser { get; set; }
        public string AvJetspeedUser { get; set; }
        public string AvComponent { get; set; }
        public string Port { get; set; }
        protected SecureString avDbPassword = new SecureString();
        protected SecureString avJetspeedDbPassword = new SecureString();

        #region getters and setters

        public void SetAvDatabasePassword(SecureString _password)
        {
            if (_password is null)
                return;

            if (avDbPassword.IsReadOnly())
            {
                avDbPassword.Dispose();
                avDbPassword = new SecureString();
                avDbPassword = _password;
                avDbPassword.MakeReadOnly();
            }
            else
            {
                avDbPassword.Clear();
                avDbPassword = _password;
                avDbPassword.MakeReadOnly();
            }
        }

        public void SetJetspeedDatabasePassword(SecureString _password)
        {
            if (_password is null)
                return;

            if (avJetspeedDbPassword.IsReadOnly())
            {
                avJetspeedDbPassword.Dispose();
                avJetspeedDbPassword = new SecureString();
                avJetspeedDbPassword = _password;
                avJetspeedDbPassword.MakeReadOnly();
            }
            else
            {
                avJetspeedDbPassword.Clear();
                avJetspeedDbPassword = _password;
                avJetspeedDbPassword.MakeReadOnly();
            }

        }

        public void SetAvDatabasePassword(string _password)
        {
            if (_password is null)
                return;

            if (avDbPassword.IsReadOnly())
            {
                avDbPassword.Dispose();
                avDbPassword = new SecureString();
                foreach (char c in _password)
                {
                    avDbPassword.AppendChar(c);
                }
                avDbPassword.MakeReadOnly();
            }
            else
            {
                avDbPassword.Clear();
                foreach (char c in _password)
                {
                    avDbPassword.AppendChar(c);
                }
                avDbPassword.MakeReadOnly();
            }

        }

        public void SetJetspeedDatabasePassword(string _password)
        {
            if (_password is null)
                return;

            if (avJetspeedDbPassword.IsReadOnly())
            {
                avJetspeedDbPassword.Dispose();
                avJetspeedDbPassword = new SecureString();
                foreach (char c in _password)
                {
                    avJetspeedDbPassword.AppendChar(c);
                }
                avJetspeedDbPassword.MakeReadOnly();
            }
            else
            {
                avJetspeedDbPassword.Clear();
                foreach (char c in _password)
                {
                    avJetspeedDbPassword.AppendChar(c);
                }
                avJetspeedDbPassword.MakeReadOnly();
            }

        }

        public string GetAvDatabasePassword()
        {
            return SecureUtils.SecureStringToString(avDbPassword);
        }

        public string GetJetspeedDatabasePassword()
        {
            return SecureUtils.SecureStringToString(avJetspeedDbPassword);
        }

        public SecureString GetAvDatabasePasswordSecure()
        {
            return avDbPassword;
        }

        public SecureString GetJetspeedDatabasePasswordSecure()
        {
            return avJetspeedDbPassword;
        }



        #endregion


        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("av.db.host: " + AvDbHost);
            sb.AppendLine("av.db.sid: " + AvDbHost);
            sb.AppendLine("av.db.username: " + AvDbHost);
            sb.AppendLine("av.db.password: " + AvDbHost);
            sb.AppendLine("av.db.port: " + AvDbHost);

            //  if(!string.IsNullOrWhiteSpace(AvJetspeedDbName))
            sb.AppendLine("av.jetspeed.db.sid: " + AvJetspeedDbName);
            // if(!string.IsNullOrWhiteSpace(AvJetspeedUser))
            sb.AppendLine("av.jetspeed.db.username: " + AvJetspeedUser);
            //if(!string.IsNullOrWhiteSpace(SecureUtils.SecureStringToString(avJetspeedDbPassword)))
            sb.AppendLine("av.jetspeed.db.password: " + SecureUtils.SecureStringToString(avJetspeedDbPassword));
            //   if(!string.IsNullOrWhiteSpace(AvJetspeedDbName))
            sb.AppendLine("av.jetspeed.db.port: " + AvDbHost);
            return sb.ToString();
        }
    }
}
