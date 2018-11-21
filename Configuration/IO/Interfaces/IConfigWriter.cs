using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rydavidson.Accela.Configuration.IO.Interfaces
{
    interface IConfigWriter
    {
        void WriteValueToConfig(string key, string value);

        
    }
}
