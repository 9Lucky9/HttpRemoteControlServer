using System.Security.Cryptography;
using System.Text;

namespace HttpRemoteControl.Library.Encryptor;

public sealed class AesEncryptor : IEncryptor
{
    public string Encrypt(string key, string text)
    {
        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        aes.GenerateIV();
        
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);
        
        using var encryptor = aes.CreateEncryptor();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        
        byte[] plainBytes = Encoding.UTF8.GetBytes(text);
        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
        csEncrypt.FlushFinalBlock();
        
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string key, string text)
    {
        byte[] bytes = Convert.FromBase64String(text);
        
        if(bytes.Length < 16)
            throw new ArgumentException("Invalid cipher text length.");
        
        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        
        byte[] iv = new byte[16];
        Array.Copy(bytes, 0, iv, 0, 16);
        aes.IV = iv;
        
        using var decryptor = aes.CreateDecryptor();
        
        using var msDecrypt = new MemoryStream(bytes, 16, bytes.Length - 16);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }
}