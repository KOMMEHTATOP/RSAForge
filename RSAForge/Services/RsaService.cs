using System.Security.Cryptography;
using System.Text;
using RSAForge.Models;

namespace RSAForge.Services;

public class RsaService : IRsaService
{
    public RsaKeyPair GenerateKeyPair(int keySize, string format)
    {
        using RSA rsa = RSA.Create(keySize);
        string privateKey;
        string publicKey;

        // Выбираем метод экспорта в зависимости от выбранного в UI формата
        if (format.Contains("PKCS#8"))
        {
            privateKey = rsa.ExportPkcs8PrivateKeyPem();
            publicKey = rsa.ExportSubjectPublicKeyInfoPem();
        }
        else if (format.Contains("PKCS#1"))
        {
            privateKey = rsa.ExportRSAPrivateKeyPem();
            publicKey = rsa.ExportRSAPublicKeyPem();
        }
        else if (format.Contains("XML"))
        {
            privateKey = rsa.ToXmlString(true);
            publicKey = rsa.ToXmlString(false);
        }
        else
        {
            privateKey = rsa.ExportPkcs8PrivateKeyPem();
            publicKey = rsa.ExportSubjectPublicKeyInfoPem();
        }

        return new RsaKeyPair
        {
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
    }

    public bool ValidateKeyPair(string publicKey, string privateKey, string testPhrase)
    {
        try
        {
            using RSA rsaPublic = RSA.Create();
            using RSA rsaPrivate = RSA.Create();

            // Пытаемся импортировать ключи (авто-определение формата в .NET работает хорошо)
            rsaPublic.ImportFromPem(publicKey);
            rsaPrivate.ImportFromPem(privateKey);

            byte[] data = Encoding.UTF8.GetBytes(testPhrase);

            // Шифруем публичным ключом
            byte[] encrypted = rsaPublic.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

            // Расшифровываем приватным ключом
            byte[] decrypted = rsaPrivate.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decrypted) == testPhrase;
        }
        catch
        {
            // Если формат ключа неверный или ключи не подходят друг другу
            return false;
        }
    }
    
    public string FormatKey(string key, bool stripHeaders, bool isSingleLine)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        string result = key;

        // 1. Убираем заголовки (PEM footer/header)
        if (stripHeaders)
        {
            string[] lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
            // Оставляем только те строки, которые не начинаются с дефисов
            var filteredLines = lines.Where(line => 
            {
                return !line.Trim().StartsWith("-----");
            });
        
            result = string.Join(Environment.NewLine, filteredLines);
        }

        // 2. Схлопываем в одну строку
        if (isSingleLine)
        {
            result = result.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        }

        return result.Trim();
    }
}