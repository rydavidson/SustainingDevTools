using rydavidson.Accela.Configuration.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rydavidson.Accela.Configuration.IO
{
    class XMLConfigWriter : IConfigWriter
    {
        public XMLConfigWriter(string _pathToConfigFile)
        {

        }

        public void WriteValueToConfig(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
