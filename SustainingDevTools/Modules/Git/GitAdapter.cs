using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using SustainingDevTools.Common;

namespace SustainingDevTools.Modules.Git
{
    class GitAdapter
    {
        private Cache cache = Cache.Instance;

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
