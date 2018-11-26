using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EnroladorStandAlone
{
    class PassVerifier
    {
        const string delim = "*";
        private static string SaltPassword(string password, string salt)
        {
            SHA512 hashAlgorithm = SHA512.Create();
            return ByteArrayToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
        public static bool AreEqual(string saltedPassword, string password, Guid userOid)
        {
            if (string.IsNullOrEmpty(saltedPassword))
                return string.IsNullOrEmpty(password);
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            string calculatedSaltedPassword = SaltPassword(password, userOid.ToString().ToUpper());
            string expectedSaltedPassword = saltedPassword.ToUpper();
            return expectedSaltedPassword.Equals(calculatedSaltedPassword);
        }

        public static string StorePassword(string password, Guid userOid)
        {
            return SaltPassword(password, userOid.ToString().ToUpper());
        }
    }
}
