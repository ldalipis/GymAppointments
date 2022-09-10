﻿using System.Text;
using System.Security.Cryptography;

namespace EnergyRoom.Helpers
{
    public static class Encryption
    {
        public static string Sha256Hash(string text)
        {
            if (text == null)
            {
                return "";
            }

            // Create a SHA256   
            using SHA256 sha256Hash = SHA256.Create();

            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            
            return builder.ToString();
        }
    }
}
