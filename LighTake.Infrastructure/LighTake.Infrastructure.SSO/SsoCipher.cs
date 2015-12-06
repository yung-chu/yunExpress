using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;

namespace SsoFramework
{
    internal class SsoCipher
    {
        internal static class Constants
        {
            internal const string Des3IV = "0A0B0C0D0E0F0A0B0C0D0E0F0A0B0C0D0E0F0A0B0C0D0E0F";
            internal const string Des3KeyName = "APP_KEY";
            internal const string Des3KeyValue = "C5C5AF0C419445C16D214638358AB0C9FA7F36B1DEAEDF8D";
            internal const char Delimiter = '$';
        }

        #region 3DES encryt/decypt methods
        public static byte[] EncryptCore(byte[] bytes)
        {
            TripleDESCryptoServiceProvider des3 = new TripleDESCryptoServiceProvider();
            //byte[] rgbKey = HexToBytes(ConfigurationManager.AppSettings[Constants.Des3KeyName]);
            byte[] rgbKey = HexToBytes(Constants.Des3KeyValue);
            return des3.CreateEncryptor(rgbKey, HexToBytes(Constants.Des3IV)).TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static byte[] DecryptCore(byte[] bytes)
        {
            TripleDESCryptoServiceProvider des3 = new TripleDESCryptoServiceProvider();
            //byte[] rgbKey = HexToBytes(ConfigurationManager.AppSettings[Constants.Des3KeyName]);
            byte[] rgbKey = HexToBytes(Constants.Des3KeyValue);
            return des3.CreateDecryptor(rgbKey, HexToBytes(Constants.Des3IV)).TransformFinalBlock(bytes, 0, bytes.Length);
        }

        private static string Encrypt(string textToEncrypt)
        {
            return BytesToHex(EncryptCore(Encoding.Default.GetBytes(textToEncrypt)));
        }

        private static string Decrypt(string textToDecrypt)
        {
            return Encoding.Default.GetString(DecryptCore(HexToBytes(textToDecrypt)));
        }

        private static bool TryDecrypt(string textToDecrypt, out string textResult)
        {
            textResult = null;
            byte[] bytes = null;
            if (!TryParseHex(textToDecrypt, out bytes))
                return false;
            textResult = Encoding.Default.GetString(DecryptCore(bytes));
            return true;
        }
        #endregion

        #region SHA-1 hash methods
        /// <summary>
        /// SHA-1
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte[] HashCore(byte[] bytes)
        {
            //MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            //return MD5.ComputeHash(bytes);
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            return sha1.ComputeHash(bytes);
        }

        public static string Hash(string textToHash)
        {
            return BytesToHex(HashCore(Encoding.Default.GetBytes(textToHash)));
        }
        #endregion

        #region Vector encrypt
        public static string EncryptVector(params string[] vector)
        {
            if(vector == null||vector.Length==0)
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i++)
            {
                if (!string.IsNullOrEmpty(vector[i]))
                {
                    sb.Append(Convert.ToBase64String(Encoding.Default.GetBytes(vector[i])));
                }
                if (i != vector.Length - 1)
                    sb.Append(Constants.Delimiter);
            }
            string digest = Hash(sb.ToString());
            sb.Append(Constants.Delimiter);
            sb.Append(digest);
            return Encrypt(sb.ToString());
        }

        public static bool TryParseVector(string encodedText, out string[] vector)
        {
            vector = null;
            if (string.IsNullOrEmpty(encodedText))
                return false;
            string textDecoded = Decrypt(encodedText);
            string[] tempVector = textDecoded.Split(Constants.Delimiter);
            if (tempVector.Length <= 1)
                return false;
            string digest = tempVector[tempVector.Length - 1];
            /*byte[] digestBytes = null;
            if (!TryParseHex(digest, out digestBytes))
                return false;*/
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tempVector.Length-1; i++)
            {
                if (!string.IsNullOrEmpty(tempVector[i]))
                {
                    sb.Append(tempVector[i]);
                }
                if (i != tempVector.Length - 2)
                    sb.Append(Constants.Delimiter);
            }
            if (string.Compare(Hash(sb.ToString()), digest) != 0)
                return false;
            vector = new string[tempVector.Length - 1];
            for (int i = 0; i < vector.Length; i++)
                vector[i] = string.IsNullOrEmpty(tempVector[i]) ? null
                    : Encoding.Default.GetString(Convert.FromBase64String(tempVector[i]));
            return true;
        }
        #endregion

        #region Helper methods
        private static byte[] HexToBytes(string hexText)
        {
            int size = (int)hexText.Length / 2;
            byte[] bytes = new byte[size];
            for (int i = 0; i < size; i++)
            {
                bytes[i] = byte.Parse(hexText.Substring(i*2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        private static bool TryParseHex(string hexText, out byte[] bytes)
        {
            bytes = null;
            if (string.IsNullOrEmpty(hexText))
                return false;
            if (hexText.Length % 2 != 0)
                return false;
            int size = (int)hexText.Length / 2;
            byte[] tmpBytes = new byte[size];
            for (int i = 0; i < size; i++)
            {
                if (!byte.TryParse(hexText.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.NumberFormatInfo.InvariantInfo, out tmpBytes[i]))
                    return false;
            }
            bytes = tmpBytes;
            return true;
        }

        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.AppendFormat(b.ToString("x2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}
