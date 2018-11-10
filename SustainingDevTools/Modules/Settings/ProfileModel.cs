using rydavidson.Accela.Configuration.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustainingDevTools.Modules.Settings
{
    class ProfileModel
    {
        public string Name { get; set; }
        public AVServerConfig DatabaseConfiguration { get; set; }
        public bool isProtected { get; set; }
    }
}
