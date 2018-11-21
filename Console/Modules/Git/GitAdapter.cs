using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Console.Common;
using System.IO;
using NLog;
using LogLevel = NLog.LogLevel;

namespace Console.Modules.Git
{
    class GitAdapter
    {
        private Cache cache = Cache.Instance;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private void InitLogger(string _logFile, bool areSettingsLoaded = true)
        {
            _logFile = $"{cache.Settings["logging_dir"]}\\{_logFile}";

            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _logFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.FromString(cache.Settings["logging_level"]), LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }

        public List<string> GetLocalRepos(string repoBaseDirectory)
        {
            List<string> localRepoList = new List<string>();
            foreach(string branchDirectory in Directory.EnumerateDirectories(repoBaseDirectory))
            {
                if (branchDirectory.ToUpperInvariant().Contains("HEAD"))
                    continue; //Ignore the HEAD branch
                localRepoList.Add(branchDirectory);
            }
            return localRepoList;
        }

        public async Task CloneRepo(string repoUrl, string workingDirectory, string targetDirectory, Action<int, int> statusReporter, Action<string, bool> spinnerAction)
        {
            var co = new CloneOptions();
            var prog = new CheckoutProgressHandler((path, completed, total) => OnProgressChanged(path, completed, total, statusReporter, spinnerAction));
            co.OnCheckoutProgress = prog;
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = cache.Settings["git_username"], Password = SecureUtils.Unprotect(cache.Settings["git_password"]) };
            string password = SecureUtils.Unprotect(cache.Settings["git_password"]);
            spinnerAction($"Downloading from {repoUrl}", true);
            string dir = await Task.Run(() => Repository.Clone(repoUrl, workingDirectory + "\\" + targetDirectory, co));
            Debug.WriteLine(DateTime.Now.ToString() + " - Cloned to " + dir);
        }

        public async Task SetBranch(string branchName, string pathToRepo)
        {
            using (var repo = new Repository(pathToRepo))
            {
                var branch = repo.Branches[branchName];

                if (branch == null)
                {

                }

                Branch currentBranch = await Task.Run(() => Commands.Checkout(repo, branch));
            }
        }

        public async Task FetchUpdatesForBranch(string pathToRepo)
        {
            string logMessage = "";
            using (var repo = new Repository(pathToRepo))
            {
                var remote = repo.Network.Remotes["origin"];
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                await Task.Run(() => Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage));
            }
            //(logMessage);
        }

        private void OnProgressChanged(string path, int completed, int total, Action<int, int> statusReporter, Action<string, bool> spinnerAction)
        {
            spinnerAction(path, false);
            statusReporter(completed, total);
            //Debug.WriteLine(DateTime.Now.ToString() + " - Path: " + path);
            //Debug.WriteLine(DateTime.Now.ToString() + " - Completed: " + completed);
            //Debug.WriteLine(DateTime.Now.ToString() + " - Total: " + total);
        }

      
    }
}
