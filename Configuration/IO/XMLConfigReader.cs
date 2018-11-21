using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rydavidson.Accela.Configuration.IO
{
    class XMLConfigReader : IConfigReader
    {
        public XMLConfigReader(string _pathToConfigFile)
        {

        }

        public IAccelaConfig Load()
        {
            throw new NotImplementedException();
        }
    }
}
