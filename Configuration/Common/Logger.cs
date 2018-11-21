using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace rydavidson.Accela.Configuration.Common
{
    public class Logger
    {
        public string LogFile { get; set; }
        public Boolean IsVerbose { get; set; }
        public Boolean IsDebug { get; set; }
        public Type CallingClass { get; set; }

        public Boolean IsEnabled { get; set; }

        protected StringBuilder lb = new StringBuilder();

        #region constructors

        public Logger()
        {
            IsEnabled = false;
        }

        //public Logger(Boolean _isEnabled) 
        //{
        //    isEnabled = _isEnabled;
        //}

        public Logger(Type _callingClass)
        {
            LogFile = _callingClass.Name + ".log";
#if DEBUG
            IsEnabled = true;
#else
            IsEnabled = false;
#endif
        }

        public Logger(string _logFile)
        {
            LogFile = _logFile;
#if DEBUG
            IsEnabled = true;
#else
            IsEnabled = false;
#endif
        }

        public Logger(string _logFile, Boolean _isDebug, Boolean _isVerbose)
        {
            LogFile = _logFile;
            IsDebug = _isDebug;
            IsVerbose = _isVerbose;
#if DEBUG
            IsEnabled = true;
#else
            IsEnabled = false;
#endif
        }

        #endregion


        public void Log(string _s)
        {
            ProcessWrite(_s + Environment.NewLine);
        }

        public void Info(string _s)
        {
            lb.AppendLine(_s);
            ProcessWrite(" - INFO: " + lb);
            lb.Clear();
        }

        public void Warn(string _s)
        {
            lb.AppendLine(_s);
            ProcessWrite(" - WARN: " + lb);
            lb.Clear();
        }

        public void Error(string _s)
        {
            lb.AppendLine(_s);
            ProcessWrite(" - ERROR: " + lb);
            lb.Clear();
        }

        public void Debug(string _s)
        {
            if (!IsDebug) return;
            lb.AppendLine(_s);
            ProcessWrite(" - DEBUG: " + lb);
            lb.Clear();
        }

        public void Trace(string _s)
        {
            if (!IsVerbose) return;
            lb.AppendLine(_s);
            ProcessWrite(" - TRACE: " + lb);
            lb.Clear();
        }

        private void ProcessWrite(string _text)
        {
            if (!IsEnabled)
                return;

            if (!File.Exists(LogFile))
                return;

            if (CallingClass != null)
                _text = CallingClass + " " + _text;

            _text = DateTime.Now.ToString(CultureInfo.InvariantCulture) + _text;
            File.AppendAllText(LogFile, _text);
        }

        //Task ProcessWrite(string text)
        //{
        //    return WriteTextAsync(logFile, text);
        //}

        //async Task WriteTextAsync(string filePath, string text)
        //{
        //    text = DateTime.Now.Tostring() + text;
        //    byte[] encodedText = Encoding.Unicode.GetBytes(text); // ran into issues with Encoding.Default so I'm using Unicode to force i18n compat
        //    using (FileStream sourceStream = new FileStream(filePath,
        //        FileMode.Append, FileAccess.Write, FileShare.None,
        //        bufferSize: 4096, useAsync: true))
        //    {
        //        await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        //    };
        //}
    }
}
