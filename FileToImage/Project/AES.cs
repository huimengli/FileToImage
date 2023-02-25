using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileToImage.Project
{
    // 定义
    //public delegate void ProgressHandler(object sender, ProgressEventArgs e);

    // 使用
    //progress?.Invoke(sender, new ProgressEventArgs(fsRead.Position, fsRead.Length));

    class AES
    {


        sealed class AESException : Exception {
            /// <summary>
            /// 错误消息
            /// </summary>
            private string message;

            public AESException()
            {

            }

            public AESException(string message)
            {
                this.message = message;
            }
            
            public new string Message
            {
                get
                {
                    return this.message;
                }
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="array">要加密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] array, string key)
        {
            key = FmtPassword(key);
            byte[] keyArray = Encoding.UTF8.GetBytes(key);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

            return resultArray;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] EncryptStr(string input,string key,byte[] IV)
        {
            key = FmtPassword(key);
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] retArray;

            using (Aes rDel = Aes.Create())
            {
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.CBC;
                rDel.IV = IV;

                ICryptoTransform encryptor = rDel.CreateEncryptor(rDel.Key,rDel.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(input);
                        }
                        //csEncrypt.Write(array, 0, array.Length);
                    }
                    retArray = msEncrypt.ToArray();
                }
            }

            return retArray;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="array"></param>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] array,string key, byte[] IV)
        {
            try
            {
                key = FmtPassword(key);
            }
            catch (AESException err)
            {
                if (err.Message== "密钥不能为空!")
                {
                    return array;
                }
                throw;
            }
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] retArray;

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.Mode = CipherMode.CBC;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                retArray = encryptor.TransformFinalBlock(array, 0, array.Length);
            }

            return retArray;
        }

        private static string FmtPassword(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new AESException("密钥不能为空!");
            }
            else if (key.Length!=32)
            {
                throw new AESException("密钥长度必须为32!");
            }
            else
            {
                return key;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="array">要解密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] array, string key)
        {
            key = FmtPassword(key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

            return resultArray;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="array">要解密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptStr(byte[] array, string key,byte[] IV)
        {
            key = FmtPassword(key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            string retArray;

            using (Aes rDel = Aes.Create())
            {
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.IV = IV;

                ICryptoTransform decryptor = rDel.CreateDecryptor();
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(array))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        //retArray = new byte[csDecrypt.Length];
                        //csDecrypt.Read(retArray, 0, retArray.Length);
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            retArray = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return retArray;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="array">要解密的 byte[] 数组</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] array, string key,byte[] IV)
        {
            try
            {
                key = FmtPassword(key);
            }
            catch (AESException err)
            {
                if (err.Message == "密钥不能为空!")
                {
                    return array;
                }
                throw;
            }
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] retArray;

            using (var aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.Mode = CipherMode.CBC;
                aes.IV = IV;

                ICryptoTransform cTransform = aes.CreateDecryptor();
                retArray = cTransform.TransformFinalBlock(array, 0, array.Length);
            }

            return retArray;
        }
    }
}
