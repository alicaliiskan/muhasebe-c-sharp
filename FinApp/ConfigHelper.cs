using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

namespace muhasebe
{
    class ConfigHelper
    {
        private static string configFilePath = "config.json"; 
        private static UserConfig _config;

        private static readonly byte[] encryptionKey = new byte[32]; // 32 bytes for AES-256
        private static readonly byte[] iv = new byte[16]; // 16 bytes for AES

        static ConfigHelper()
        {
          
            byte[] tempKey = Encoding.UTF8.GetBytes("Yt9vRz8KqPz1MfL3XnHjWs0YdTk5E"); // 32 karakter şifre
            Array.Copy(tempKey, encryptionKey, Math.Min(tempKey.Length, 32));

            byte[] tempIV = Encoding.UTF8.GetBytes("IvG7kPz3VwQ1Xf9Y"); // 16 karakter
            Array.Copy(tempIV, iv, Math.Min(tempIV.Length, 16));
        }

        public static UserConfig Config
        {
            get
            {
                if (_config == null)
                {
                    LoadConfig(); 
                }
                return _config;
            }
        }

        public static void LoadConfig()
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    string encryptedJson = File.ReadAllText(configFilePath);
                    string decryptedJson = Decrypt(encryptedJson);
                    _config = JsonSerializer.Deserialize<UserConfig>(decryptedJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Konfigürasyon yüklenirken hata: " + ex.Message);
                    _config = new UserConfig(); 
                }
            }
            else
            {
                _config = new UserConfig(); 
            }
        }

        public static void SaveConfig()
        {
            try
            {
                string json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                string encryptedJson = Encrypt(json);
                File.WriteAllText(configFilePath, encryptedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Konfigürasyon kaydedilirken hata: " + ex.Message);
            }
        }

     
        private static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }


        private static string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = encryptionKey;
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Şifre çözme hatası: " + ex.Message, ex);
            }
        }
    }

    public class UserConfig
    {
        public string uygulamaSifre { get; set; } = "";
        public string EPosta { get; set; } = "";
        public string uygulamaAdi { get; set; } = "";
        public string smtpHost { get; set; } = "";
    }
}
