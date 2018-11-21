using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using rydavidson.Accela.Configuration.Handlers;

namespace rydavidson.Accela.Configuration.Common
{
    public class AaUtil
    {
        private Logger logger;
        private ExceptionHandler exceptionHandler;
        private RegistryKey accelaBaseKey;
        private RegistryKey instanceKey;
        private string aaInstallDir;
        private bool isOverriddenInstallDir;

        #region constructors
        public AaUtil(string _logfile)
        {
            logger = new Logger(_logfile);
            logger.IsEnabled = true;
            exceptionHandler = new ConfigurationExceptionHandler();
            exceptionHandler.SetLogger(logger);
        }

        public AaUtil(Logger _logger)
        {
            logger = _logger;
            logger.IsEnabled = true;
            exceptionHandler = new ConfigurationExceptionHandler();
            exceptionHandler.SetLogger(logger);
        }

        public AaUtil()
        {
            logger = new Logger();
            exceptionHandler = new ConfigurationExceptionHandler();
            exceptionHandler.SetLogger(logger);
        }
        #endregion


        #region registry

        private RegistryKey GetAccelaBaseKey()
        {
            RegistryKey hklmReg;

            if (accelaBaseKey != null)
                return accelaBaseKey;

            try
            {
                hklmReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                try
                {
                    accelaBaseKey = hklmReg.OpenSubKey(@"SOFTWARE\WOW6432Node\Accela Inc.", RegistryKeyPermissionCheck.ReadSubTree);
                    return accelaBaseKey;
                }
                catch (Exception e)
                {
                    exceptionHandler.HandleException(e, "Unable to access Accela registry key");
                }
            }
            catch (Exception e)
            {
                exceptionHandler.HandleException(e, "Unable to open HKLM registry key");
            }
            return accelaBaseKey;
        }

        private RegistryKey GetInstanceKey(string _version, string _instance)
        {

            if ((instanceKey != null) && ((string)instanceKey.GetValue("InstanceName") == _instance))
                return instanceKey;

            RegistryKey reg = GetAccelaBaseKey();

            try
            {
                instanceKey = reg.OpenSubKey(string.Format(@"AA Base Installer\{0}\{1}", _version, _instance), RegistryKeyPermissionCheck.ReadSubTree);
            }
            catch (Exception e)
            {
                exceptionHandler.HandleException(e, "Unable to get install directory from registry");
            }

            return instanceKey;
        }

        #endregion

        public List<string> GetAaVersions()
        {
            //if(GetAccelaBaseKey() == null)
            //{
            //    StringBuilder err = new StringBuilder();
            //    err.AppendLine("Couldn't open Accela Base Key");
            //    logger.LogError(err.ToString());
            //    return null;
            //}
            return new List<string>(GetAccelaBaseKey().OpenSubKey(@"AA Base Installer").GetSubKeyNames());
        }

        public List<string> GetInstancesForVersion(string _versionToCheck)
        {
            List<string> instances = new List<string>();
            try
            {
                RegistryKey accelaBaseKey = GetAccelaBaseKey();
                List<string> versions = GetAaVersions();
                foreach (string version in versions)
                {
                    if (version.Trim() == _versionToCheck.Trim())
                    {
                        foreach (string instance in GetAccelaBaseKey().
                            OpenSubKey(@"AA Base Installer\" + version).GetSubKeyNames())
                        {
                            instances.Add(instance);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                exceptionHandler.HandleException(uaex, "Access Denied getting instances for version " + _versionToCheck);
            }
            catch (Exception e)
            {
                exceptionHandler.HandleException(e, "Unable to get instances for version");
            }

            return instances;
        }

        public string GetAaInstallDir(string _version, string _instance)
        {
            if(aaInstallDir != null)
                return aaInstallDir;
            if (isOverriddenInstallDir)
                throw new Exception("The install directory is overridden but no install directory has been provided");

            RegistryKey reg = GetAccelaBaseKey();
            string installDir = "";
            if (reg != null && _version != null && _instance != null)
            {
                try
                {
                    instanceKey = reg.OpenSubKey(string.Format(@"AA Base Installer\{0}\{1}", _version, _instance), RegistryKeyPermissionCheck.ReadSubTree);
                    installDir = instanceKey.GetValue("InstallDir").ToString();
                }
                catch (Exception e)
                {
                    exceptionHandler.HandleException(e, "Unable to get install directory from registry");
                }
            }
            //if (installDir != "")
            aaInstallDir = installDir;
            return aaInstallDir;
        }

        public void SetAaInstallDir(string path)
        {
            if (Directory.Exists(path))
            {
                aaInstallDir = path;
                isOverriddenInstallDir = true;
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory {path} was not found");
            }
        }
        public List<string> GetAaInstalledComponents(string _version, string _instance)
        {
            RegistryKey reg = GetAccelaBaseKey();
            string components = GetInstanceKey(_version, _instance).GetValue("InstallComponents").ToString();
            List<string> compList = new List<string>();
            foreach(string comp in components.Split(','))
            {
                compList.Add(comp);
            }
            return compList;
        }
        public Dictionary<string, string> GetAaConfigFilePaths(string _version, string _instance)
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            List<string> components = GetAaInstalledComponents(_version, _instance);
            string installDir = GetAaInstallDir(_version, _instance);

            foreach (string comp in components)
            {
                string stemp = string.Format(@"{0}\{1}\conf\av\ServerConfig.properties", installDir, comp);
                if (File.Exists(stemp))
                    paths.Add(comp, stemp);
            }
            return paths;
        }
    }
}
