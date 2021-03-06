﻿using System;
using System.Security.Cryptography;

namespace Support_Your_Locals.Cryptography
{
    public class HashCalculator
    {

        private byte[] salt = new byte[16];

        public string PassHash(string password)
        {
            if (password == null) password = "";
            new RNGCryptoServiceProvider().GetBytes(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public bool IsGoodPass(string userpass, string loginpass)
        {
            if (userpass == null) userpass = "";
            if (loginpass == null) loginpass = "";
            bool goodpass = false;
            string savedPasswordHash = userpass;
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(loginpass, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] == hash[i]) goodpass = true;
                else
                {
                    goodpass = false;
                    break;
                }
            }
            return goodpass;
        }

    }
}
