using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DevExpress.Persistent.Base;

namespace EnroladorStandAlone
{
    class PassVerifier
    {
        const string delim = "*";

        public static string StorePassword(string password, Guid userOid)
        {
            return SaltPassword(password, userOid.ToString().ToUpper());
        }

        public static bool AreEqual(string saltedPassword, string password, Guid userOid) {
            if (string.IsNullOrEmpty(saltedPassword))
                return string.IsNullOrEmpty(password);
            if (string.IsNullOrEmpty(password)) {
                return false;
            }
            int delimPos = saltedPassword.IndexOf(delim);
            if (delimPos <= 0) {
                return saltedPassword.Equals(password);
            } else {
                string calculatedSaltedPassword = SaltPassword(password, saltedPassword.Substring(0, delimPos));
                string expectedSaltedPassword = saltedPassword.Substring(delimPos + delim.Length);
                if (expectedSaltedPassword.Equals(calculatedSaltedPassword)) {
                    return true;
                }
                return expectedSaltedPassword.Equals(SaltPassword(password, "System.Byte[]"));
            }
        }

        private static string SaltPassword(string password, string salt)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password))); ;
        }
    }
}
