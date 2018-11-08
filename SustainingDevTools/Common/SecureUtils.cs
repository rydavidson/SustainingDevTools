using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SustainingDevTools.Common
{
    public static class SecureUtils
    {
        public static string SecureStringToString(SecureString _value)
        {
            return new System.Net.NetworkCredential(string.Empty, _value).Password;
        }

        public static string Protect(string stringToEncrypt, string optionalEntropy = null)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(stringToEncrypt)
                    , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                    , DataProtectionScope.LocalMachine));
        }

        //public static string Protect(SecureString stringToEncrypt, string optionalEntropy = null)
        //{
        //    return Convert.ToBase64String(
        //        ProtectedData.Protect(
        //            Encoding.UTF8.GetBytes(SecureStringToString(stringToEncrypt))
        //            , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
        //            , DataProtectionScope.LocalMachine));
        //}

        public static string Unprotect(string encryptedString, string optionalEntropy = null)
        {

            byte[] bytes = Convert.FromBase64String(encryptedString);
            byte[] unprotected = ProtectedData.Unprotect(bytes, null, DataProtectionScope.LocalMachine);
            string pass = Encoding.UTF8.GetString(unprotected);


            

            return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedString)
                    , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                    , DataProtectionScope.LocalMachine));
        }
    }
}
