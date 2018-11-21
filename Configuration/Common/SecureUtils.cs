using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace rydavidson.Accela.Configuration.Common
{
    public static class SecureUtils
    {
        public static String SecureStringToString(SecureString _value)
        {
            return new System.Net.NetworkCredential(string.Empty, _value).Password;
        }
    }
}
